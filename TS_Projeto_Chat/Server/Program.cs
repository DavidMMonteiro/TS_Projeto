/*
    INFO
    Servidor para criar um chat cliente - servidor
    Este sistema irá permitir dois ou mais clientes acceder as suas contas no servirdor y 
    iniciar um chat com um ou varios clientes do servidor.

    OWNER INFO
    Class: 4515 Curso Tecnico Superior Profissional de Programacao de Sistemas de Informacao
    UC: Tópicos de Segurança
    Student(s) number: 2211849
    Creator(s): David Machado Monteiro
*/
using System;
using System.Net;
using System.Net.Sockets;
using EI.SI;
using TS_Chat;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            LogController logger = new LogController();
            string name = "Server";

            int PORT = 10000;
            //
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            TcpListener listener = new TcpListener(endPoint);
            Dictionary<string, TcpClient> clientsDictionary = new Dictionary<string, TcpClient>();
            //
            listener.Start();
            //
            logger.consoleLog("Server Start", name);
            //
            while (true)
            {
                try
                {
                    //Open client conexión
                    TcpClient client = listener.AcceptTcpClient();
                    //Get client data send
                    NetworkStream networkStream = client.GetStream();
                    ProtocolSI protocolSI = new ProtocolSI();
                    //Gets the client data
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    //Get the client data from the protocol
                    string dataFromClient = protocolSI.GetStringFromData();
                    byte[] ack;
                    logger.consoleLog("Connection try!", name);
                    //Check if new user it's being created
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_1)
                    {
                        //Create new user
                        if (CreateUser(dataFromClient))
                        {
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, "True");
                            networkStream.Write(ack, 0, ack.Length);
                        }
                        else
                        {
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, "False");
                            networkStream.Write(ack, 0, ack.Length);
                        }
                    }
                    //Check User loggin
                    else if(checkUser(dataFromClient))
                    {
                        //Console info
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "True");
                        networkStream.Write(ack, 0, ack.Length);
                        //Create Cliente Handler
                        clientsDictionary.Add(dataFromClient.Split('$')[0], client);
                        ClientHandler clientHandler = new ClientHandler(client, dataFromClient.Split('$')[0], clientsDictionary);
                        clientHandler.Handle();
                    }
                    else
                    {
                        logger.consoleLog("User not accepted", "Server");
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "False");
                        networkStream.Write(ack, 0, ack.Length);
                    }
                }catch (Exception ex){
                    logger.consoleLog(ex.Message, name);
                }
                
            }
        
        }
        //Create new user
        private static bool CreateUser(string dataFromClient)
        {
            LogController logController = new LogController();
            //Get username from string
            string username = dataFromClient.Split('$')[0];
            //Get salt size
            int saltSize = Convert.ToInt32(dataFromClient.Split('$')[1]);
            //Get Salt from string 
            byte[] salt = Encoding.UTF8.GetBytes(dataFromClient.Split('$')[2].Substring(0, saltSize));
            //Get Hash from string
            byte[] hash = Encoding.UTF8.GetBytes(dataFromClient.Split('$')[2]);

            using (ChatBDContainer chatBDContainer = new ChatBDContainer())
            {
                // Validate it doesn't exist
                if (chatBDContainer.UsersSet.ToList().FindAll(u => u.Username == username).Count > 0)
                    return false;

                //Create new user
                Users user = new Users(username, hash, salt);
                try
                {
                    //Add new user to DataBase
                    chatBDContainer.UsersSet.Add(user);
                    chatBDContainer.SaveChanges();
                    logController.consoleLog($"New user {user.Username} created", "Server");
                    return true;
                }
                catch (Exception ex)
                {
                    logController.consoleLog(ex.Message, "Server");
                    return false;
                }
            }            
        }

        private static bool checkUser(string user_info)
        {
            string check_Username = user_info.Split('$')[0];
            byte[] check_SaltedPasswordHash = Encoding.UTF8.GetBytes(user_info.Split('$')[1]);

            //Get the user data list
            List<Users> user_list = LoadUsers();

            //Check if the list have some data, if not exit
            if (user_list == null || user_list.Count == 0) 
                return false; 

            //Get the user and check the salted hash
            return user_list.Find(u => u.Username.Equals(check_Username)).checkedSaltPassword(check_SaltedPasswordHash);
        }

        // Carrega a informação dos utilizadores
        private static List<Users> LoadUsers()
        {
            LogController logger = new LogController();
            // Inicialização do chatContainer
            ChatBDContainer chatBDContainer = new ChatBDContainer();
            // Cria a lista de utilizadores
            var users = chatBDContainer.UsersSet;
            logger.consoleLog("Get users from DataBase", "Server");
            // Lee a informação do ficheiro e divide
            foreach(Users user in users)
                Console.Write("Hi! I'm user: " + user.Username);
            //Retorna a lista de utilizadores
            return null;
        }
    } 	
}


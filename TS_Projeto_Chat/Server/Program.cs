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
using System.Collections.Generic;
using System.IO;
using TS_Chat;

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
                    if (!checkUser(dataFromClient))
                    {
                        logger.consoleLog("User not accepted", "Server");
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "False");
                        networkStream.Write(ack, 0, ack.Length);
                    }
                    else
                    {
                        //Console info
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "True");
                        networkStream.Write(ack, 0, ack.Length);
                        //Create Cliente Handler
                        clientsDictionary.Add(dataFromClient.Split('$')[0], client);
                        ClientHandler clientHandler = new ClientHandler(client, dataFromClient.Split('$')[0], clientsDictionary);
                        clientHandler.Handle();
                    }
                }catch (Exception ex){
                    logger.consoleLog(ex.Message, name);
                }
                
            }
        
        }

        private static bool checkUser(string user_info)
        {
            //Get the user data list
            List<User> user_list = LoadUsersFiles();
            //Check if the list have some data, if not exit
            if (user_list == null || user_list.Count == 0)
                return false;

            //Check for the user name and password 
            foreach (User user in user_list)
                if (user.Username == user_info.Split('$')[0] && user.checkPassword(user_info.Split('$')[1]))
                    return true;

            return false;
        }

        // Carrega a informação dos utilizadores a partir de ficheiros txt
        // (sustituir por base de dados)
        private static List<User> LoadUsersFiles()
        {
            // Cria a lista de utilizadores
            List<User> users = new List<User>();
            // Lee a informação do ficheiro e divide
            foreach(string user in File.ReadAllText("users.txt").Split(';'))
                // Cria um novo utilizador por linha
                users.Add(new User(user.Split('$')[0], user.Split('$')[1]));
            //Retorna a lista de utilizadores
            return users;
        }
    } 	
}


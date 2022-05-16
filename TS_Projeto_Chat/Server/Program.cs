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
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Helper helper = new Helper();
            string name = "Server";

            int PORT = 10000;
            //
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();
            //
            helper.consoleLog("Server Start", name);
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
                    helper.consoleLog("Connection try!", name);
                    if (!checkUser(dataFromClient))
                    {
                        helper.consoleLog("User not accepted", "Server");
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "False");
                        networkStream.Write(ack, 0, ack.Length);
                    }
                    else
                    {
                        //Console info
                        helper.consoleLog(dataFromClient.Split('$')[0] + " connected", name);
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "True");
                        networkStream.Write(ack, 0, ack.Length);
                        //Create Cliente Handler
                        ClientHandler clientHandler = new ClientHandler(client, dataFromClient.Split('$')[0]);
                        clientHandler.Handle();
                    }
                }catch (Exception ex){
                    helper.consoleLog(ex.Message, name);
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
    
    // Thread para do client
    class ClientHandler
    {
        Helper helper = new Helper();
        private TcpClient client;
        private string clientName;

        // Construtor do ClientHandler
        public ClientHandler(TcpClient client, string clientName)
        {
            this.client = client;
            this.clientName = clientName;
        }

        // Inizializa a nova Therad
        public void Handle()
        {
            Thread thread = new Thread(threadHandler);
            thread.Start();
        }

        // Função que vai comprir a nova thread
        private void threadHandler()
        {
            // Vai establecer a ligação do cliente
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            // Enquando a mensagem do cliente não for EOT
            while(protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    // Vai ler a mensagem do cliente
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    string output;
                    /* 
                    Filtra o tipo de mensagem recebida
                    A estrutura básica de cada mensagem consiste em:
                    - Lee a mensagem
                    - Escreve na consola do servidor
                    - Envia mensagem ao cliente a confirmar que recebeu a mensagem
                    */
                    switch (protocolSI.GetCmdType())
                    {
                        // Se for do tipo DATA
                        case ProtocolSICmdType.DATA:
                            output = protocolSI.GetStringFromData();
                            helper.consoleLog(output, this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = this.clientName + " exit chat";
                            helper.consoleLog(output, this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                        // Caso o tipo de mensagem enviada não for reconhecida
                        default:
                            output = "Protocol Type not know";
                            helper.consoleLog(output, this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    helper.consoleLog(ex.Message, this.clientName);
                    break;
                }
                catch (IOException ex) // Excepção de Socket
                {
                    helper.consoleLog("Socket error: \r\n\t" + ex.Message, this.clientName);
                    break;
                }
                catch (Exception ex) // Excepção desconhecida 
                {
                    helper.consoleLog("Uncommon error\r\n" + ex.Message, this.clientName);
                    break;
                }
            }

            networkStream.Close();
            client.Close();
        }
    }

    // Class User para guardar a informação do utilizador
    class User
    {

        public String Username { get; set; }
        private String Password;

        //User constructor
        public User(String user, String password)
        {
            this.Username = user;
            this.Password = password;
        }

        //Valida a password do utilizador
        public bool checkPassword(string tmpPassword)
        {
            return (tmpPassword != null && tmpPassword == this.Password);
            
        }
    }

    // Class helper para enviar mensagens a consola
    class Helper
    {
        public void consoleLog(string msg, string owner)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + owner + ": " + msg);
        }
    }
}


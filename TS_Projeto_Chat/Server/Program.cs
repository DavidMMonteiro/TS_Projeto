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
            List<TcpClient> clientsList = new List<TcpClient>();
            //
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
                        ack = protocolSI.Make(ProtocolSICmdType.ACK, "True");
                        networkStream.Write(ack, 0, ack.Length);
                        //Create Cliente Handler
                        clientsList.Add(client);
                        ClientHandler clientHandler = new ClientHandler(client, dataFromClient.Split('$')[0], clientsList);
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

        private static List<User> LoadUsersFiles()
        {
            List<User> users = new List<User>();
            string user_info = File.ReadAllText("users.txt");
            foreach(string user in user_info.Split(';'))
                users.Add(new User(user.Split('$')[0], user.Split('$')[1]));

            return users;
        }
    } 
    
    class ClientHandler
    {
        Helper helper = new Helper();
        private TcpClient client;
        private string clientName;
        private List<TcpClient> clientsList;
        private Thread thread;


        public ClientHandler(TcpClient client, string clientName, List<TcpClient> clientsList)
        {
            this.client = client;
            this.clientName = clientName;
            this.clientsList = clientsList;
            ProtocolSI protocol = new ProtocolSI();
            string msg = $"{this.clientName} join the chat";
            byte[] ack = protocol.Make(ProtocolSICmdType.DATA, msg);
            helper.consoleLog(msg);
            this.broadCast(ack, this.client.GetStream());
        }


        public void Handle()
        {
            thread = new Thread(threadHandler);
            thread.Start();
            clientsList.Remove(client);
        }

        private void threadHandler()
        {
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            //
            while(protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    string output;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            output = protocolSI.GetStringFromData();
                            helper.consoleLog(output, this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.DATA, $"({this.clientName}): {output}");
                            broadCast(ack, networkStream);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = this.clientName + " left the chat";
                            helper.consoleLog(output);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, output);
                            broadCast(ack, networkStream);
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    helper.consoleLog(ex.Message, this.clientName);
                    break;
                }
                catch (IOException ex)
                {
                    helper.consoleLog("Socket error: \r\n\t" + ex.Message, this.clientName);
                    break;
                }
                catch (Exception ex)
                {
                    helper.consoleLog("Uncommon error\r\n" + ex.Message, this.clientName);
                    break;
                }
            }

            networkStream.Close();
            client.Close();
        }

        private void broadCast(byte[] data, NetworkStream networkStream)
        {
            foreach (TcpClient clientList in this.clientsList)
                networkStream.Write(data, 0, data.Length);
        }
    }

    class User
    {

        public String Username { get; set; }
        private String Password;

        public User(String user, String password)
        {
            this.Username = user;
            this.Password = password;
        }

        public bool checkPassword(string tmpPassword)
        {
            return (tmpPassword != null && tmpPassword == this.Password);
            
        }
    }

    class Helper
    {
        public void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + msg);
        }
        public void consoleLog(string msg, string owner)
        {
            Console.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + "(" + owner + ")" + ": " + msg);
        }

    }
}


﻿/*
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
    
    // Thread para do client
    class ClientHandler
    {
        private LogController logger = new LogController();
        private TcpClient client;
        private string clientName;
        private Dictionary<string, TcpClient> ClientsDictionary;
        private Thread thread;

		// Construtor do ClientHandler
        public ClientHandler(TcpClient client, string clientName, Dictionary<string, TcpClient> clientsDictionary)     
        {
            this.client = client;
            this.clientName = clientName;
            this.ClientsDictionary = clientsDictionary;
            ProtocolSI protocol = new ProtocolSI();
            string msg = $"{this.clientName} join the chat";
            byte[] ack = protocol.Make(ProtocolSICmdType.DATA, msg);
            logger.consoleLog(msg);
            this.broadCast(ack);
        }

        // Inizializa a nova Therad
        public void Handle()
        {
            thread = new Thread(threadHandler);
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
                            logger.consoleLog(output, this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.DATA, $"({this.clientName}): {output}");
                            broadCast(ack);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = this.clientName + " left the chat";
                            logger.consoleLog(output);
                            ack = protocolSI.Make(ProtocolSICmdType.EOT, output);
                            broadCast(ack);
                            break;
                    }
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
                        break;
                }
                catch (SocketException ex)
                {
                    logger.consoleLog(ex.Message, this.clientName);
                    break;
                }
                catch (IOException ex) // Excepção de Socket
                {
                    logger.consoleLog("Socket error: " + ex.Message, this.clientName);
                    break;
                }
                catch (Exception ex) // Excepção desconhecida 
                {
                    logger.consoleLog("Uncommon error: " + ex.Message, this.clientName);
                    break;
                }
            }
            //Termina a stream do client
            networkStream.Close();
            //Termina o TcpClient
            client.Close();
            //Remove a ligação com o cliente da lista
            ClientsDictionary.Remove(this.clientName);
        }

        // Envia mensagem a todas a ligações
        private void broadCast(byte[] data)
        {
            // Loop por cada cliente
            foreach(KeyValuePair<string,TcpClient> client in ClientsDictionary) 
                // Envia a mensagem ao cliente
                client.Value.GetStream().Write(data, 0, data.Length);   
        }

        // Envia mensagem a uma lista de clientes
        private void multiCast(byte[] data, List<string> destenies)
        {
            // Itenera pelos nomes destino
            foreach(string desteny in destenies)
                // Valida se o nome destino existe no dicionario
                if(ClientsDictionary.TryGetValue(desteny, out TcpClient client))
                    // Envia a mensagem ao cliente especifico
                    client.GetStream().Write(data, 0, data.Length);
        }

        // Envia mensagem a so um client
        private void unicast(byte[] data, string desteny)
        {
            // Valida se o destino existe no dictionario
            if(ClientsDictionary.TryGetValue(desteny, out TcpClient client))
                // Envia ao destino se existir
                client.GetStream().Write(data, 0, data.Length);
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

	// Class LogController para enviar mensagens a consola
    class LogController    
    {
        //Mensagem simple para a consola
        public void consoleLog(string msg)
        {
            // Constroe a mensagem
            msg = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + msg;
            // Guarda a msg no ficheiro
            this.logFile(msg);
            // Escreve a msg na consola
            Console.WriteLine(msg);
        }

        //Mensagem composta para a consola
        public void consoleLog(string msg, string owner)
        {
            // Constroe a mensagem
            msg = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + "(" + owner + ")" + ": " + msg;
            // Guarda a msg no ficheiro
            this.logFile(msg);
            // Escreve a msg na consola
            Console.WriteLine(msg);
        }
        
        //Cria e guarda os logs do servidor
        private void logFile(string msg)
        {
            try
            {
                // Constroe o nome do ficheiro
                string pathFile = "chat_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
                // Valida se o ficheiro existe
                if (!File.Exists(pathFile))
                    File.Create(pathFile);
                // Guarda a informação no ficheiro
                File.AppendAllText(pathFile, "\r\n" + msg);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}


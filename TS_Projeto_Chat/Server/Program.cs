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
using EI.SI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            Dictionary<Users, TcpClient> clientsDictionary = new Dictionary<Users, TcpClient>();
            //
            listener.Start();
            //
            logger.consoleLog("Server Start", name);
            //
            while (true)
            {

                //Open client conexión
                TcpClient client = listener.AcceptTcpClient();
                //Get client data send
                NetworkStream networkStream = client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                //Initialize the encryptor
                Cryptor cryptor = new Cryptor();
                //Gets the client data
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                //Get the client data from the protocol
                string dataFromClient = protocolSI.GetStringFromData();
                byte[] ack;
                logger.consoleLog("Connection try!", name);
                    //Check if new user it's being created
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_1)
                    {
                        
                        //Crea um novo utilizador e valida se foi efetuado com sucesso
                        if (CreateUser(dataFromClient))
                        {
                            //Returna como sucesso ao cliente
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("True$"));
                            networkStream.Write(ack, 0, ack.Length);
                        }
                        else
                        {
                            //Returna como fallo ao cliente e o tipo de fallo
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("False$Erro na criação da conta\nValide os seus dados e tente novamente."));
                            networkStream.Write(ack, 0, ack.Length);
                        }
                    }
                    else if (protocolSI.GetCmdType() == ProtocolSICmdType.ACK)
                    {
                        try
                        {
                            /*
                             * Valida que a password do utilizador esta certa 
                             * Caso estiver correto, retorna a informação do cliente
                             * caso contrario return como null e devolve a informação ao cliente
                             */
                            Users new_user = CheckUser(dataFromClient, clientsDictionary);
                            //
                            if (new_user != null)
                            {
                                //Console info
                                ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("True$"));
                                networkStream.Write(ack, 0, ack.Length);
                                //Create Cliente Handler
                                clientsDictionary.Add(new_user, client);
                                //Criar uma nova thread para o cliente
                                ClientHandler clientHandler = new ClientHandler(client, new_user, clientsDictionary);
                                clientHandler.Handle();
                            }
                            else
                            {
                                //Console info
                                logger.consoleLog("User not accepted", "Server");
                                ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("False$Username ou Password erradas\nVerifique os seus dados"));
                                networkStream.Write(ack, 0, ack.Length);
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            //Caso um utilizador tentar fazer loggin com a conta já iniciada
                            logger.consoleLog("Two same accounts logging try", name);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("False$" + ex.Message));
                            networkStream.Write(ack, 0, ack.Length);
                        }
                        catch (InvalidOperationException ex)
                        {
                            //Caso não encontrar ao utilizador inserido
                            logger.consoleLog(ex.Message, name);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("False$Username not found."));
                            networkStream.Write(ack, 0, ack.Length);
                        }
                        catch (Exception ex)
                        {
                            //Caso acontecer um erro unknow no loggin manda essa informação ao utilizador
                            logger.consoleLog(ex.Message, name);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK, cryptor.SingData("False$Unknow logging problem from the server. Sorry...\nTry again later..."));
                            networkStream.Write(ack, 0, ack.Length);
                        }
                    }

            }

        }

        //Create new user
        private static bool CreateUser(string dataFromClient)
        {
            //Initialize Decryptor
            Cryptor cryptor = new Cryptor();
            //Desencripta a mensagem
            string message = cryptor.VerifyData(dataFromClient);
            //
            LogController logController = new LogController();

            if (message.Split('$').Length != 2)
                return false;

            //Get username from string
            string username = message.Split('$')[0];
            //Get Salt from string 
            string password = message.Split('$')[1];

            using (ChatBDContainer chatBDContainer = new ChatBDContainer())
            {
                // Validate it doesn't exist
                if (chatBDContainer.UsersSet.Any(x => x.Username == username))
                    return false;
                //Genera um salt aleatorio
                byte[] salt = cryptor.GenerateSalt();
                //Genera o salted hash
                byte[] hash = cryptor.GenerateSaltedHash(password, salt);
                //Create new user
                Users user = new Users(username, salt, hash);
                try
                {
                    //Add new user to DataBase
                    chatBDContainer.UsersSet.Add(user);
                    chatBDContainer.SaveChanges();
                    logController.consoleLog($"New account {user.Username} created with success!", "Server");
                    return true;
                }
                catch (Exception ex)
                {
                    logController.consoleLog(ex.ToString(), "Server");
                    return false;
                }
            }
        }

        private static Users CheckUser(string user_info, Dictionary<Users, TcpClient> clientsDictionary)
        {
            //Initialize Decryptor
            Cryptor cryptor = new Cryptor();
            //Desencripta a mensage
            string message = cryptor.VerifyData(user_info);
            //
            if (message == null)
                return null;
            //
            LogController logController = new LogController();
            //Get loggin data
            string check_Username = message.Split('$')[0];
            //
            string password = message.Split('$')[1];

            // Inicialização do chatContainer
            ChatBDContainer chatBDContainer = new ChatBDContainer();
            //Get the user data 
            Users user = chatBDContainer.UsersSet.ToList().Where(u => u.Username == check_Username).First();
            //
            //Valida se o utilizador esta no sistema
            if (user == null)
                return null;
            //
            Cryptor crypofer = new Cryptor();
            //Cria a hash com a pase que foi enviada
            byte[] chech_hash = crypofer.GenerateSaltedHash(password, user.Salt);
            
            //TESTE LOGS
            //logController.consoleLog("Client message: " + message, "Server");
            //logController.consoleLog("Server User data: " + "\n- " + user.Username + "\n- " + user.Salt + "\n- " + user.SaltedPasswordHash, "Server");

            //Valida que as hash seijam iguais
            if (user.checkedSaltPassword(chech_hash))
            {
                //Valida se existem clientes ativos
                if (clientsDictionary.Count == 0)
                    return user;
                //Valida se o novo cliente já esta loggado
                else if (!clientsDictionary.Keys.Any(c => c.IdUser == user.IdUser))
                    return user;
                //Caso contrario, mando erro ao cliente
                else
                    throw new ArgumentException("Client logged right now\nClose another open session to open this one...");
            }
            else
                return null;
        }
    }
}


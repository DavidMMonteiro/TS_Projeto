﻿using EI.SI;
using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace TS_Chat
{
    // Thread para do client
    public class ClientHandler
    {
        private LogController logger = new LogController();
        private TcpClient tcpClient;
        private Users client;
        private Dictionary<Users, TcpClient> ClientsDictionary;
        private Thread thread;

        // Construtor do ClientHandler
        public ClientHandler(TcpClient tcpClient, Users client, Dictionary<Users, TcpClient> clientsDictionary)
        {
            this.tcpClient = tcpClient;
            this.client = client;
            this.ClientsDictionary = clientsDictionary;
            ProtocolSI protocol = new ProtocolSI();
            Cryptor cryptor = new Cryptor();
            string msg = $"{this.client.Username} join the chat";
            logger.consoleLog(msg);
            byte[] ack = protocol.Make(ProtocolSICmdType.DATA, cryptor.GerarMensagem(msg));
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
            NetworkStream networkStream = this.tcpClient.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            // Enquando a mensagem do cliente não for EOT
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                string error = null;
                try
                {
                    // Vai ler a mensagem do cliente
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    string output;
                    //Initialize the encryptor
                    Cryptor cryptor = new Cryptor();
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
                            logger.consoleLog("Saving message", this.client.Username);
                            saveMessage(output);
                            ack = protocolSI.Make(ProtocolSICmdType.DATA, output);
                            broadCast(ack);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = this.client.Username + " left the chat";
                            logger.consoleLog(output);
                            output = cryptor.GerarMensagem(output);
                            ack = protocolSI.Make(ProtocolSICmdType.EOT, output);
                            broadCast(ack);
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            logger.consoleLog("Sending chat to request", "Server");
                            List<Mensagens> chats = LoadChat();
                            if (chats.Count > 0)
                            {
                                foreach (Mensagens chat in chats) 
                                {
                                    logger.consoleLog("Serializing chat to JSON", "Server");
                                    try
                                    {
                                        string msg = JsonConvert.SerializeObject(chat);
                                        msg = cryptor.GerarMensagem(msg);
                                        ack = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, msg);
                                        networkStream.Write(ack, 0, ack.Length);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.consoleLog($"Serializing error: \n- Error Serializing message:{chat.IdMensagem}\n" + ex.Message, "Server");
                                    }
                                }
                            }
                            else
                            {
                                logger.consoleLog("No chat found", "Server");
                            }
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    error = $"(Server): Error processing message.\n - Socket error\nGet in contact with administration";
                    logger.consoleLog(ex.Message, this.client.Username);

                }
                catch (IOException ex) // Excepção de Socket
                {
                    error = $"(Server): Error processing message.\n - IOException\nGet in contact with administration";
                    logger.consoleLog("Socket error: " + ex.Message, this.client.Username);
                }
                catch (Exception ex) // Excepção desconhecida 
                {
                    error = $"(Server): Error processing message.\n - Unknow error catch\nGet in contact with administration";
                    logger.consoleLog("Uncommon error: " + ex.Message, this.client.Username);
                }
                //
                if (!string.IsNullOrEmpty(error))
                {
                    broadCast(protocolSI.Make(ProtocolSICmdType.EOT, error));
                    break;
                }
                
            }
            //Termina a stream do client
            networkStream.Close();
            //Termina o TcpClient
            tcpClient.Close();
            //Remove a ligação com o cliente da lista
            ClientsDictionary.Remove(this.client);
        }

        private static List<Mensagens> LoadChat()
        {
            LogController logger = new LogController();
            logger.consoleLog("Loading chat", "Server");
            using (ChatBDContainer chatBDContainer = new ChatBDContainer())
            {
                logger.consoleLog("Sorting chat by date time", "Server");
                List<Mensagens> mensagens = chatBDContainer.MensagensSet.ToList();
                mensagens.Sort((x, y) => x.dtCreation.CompareTo(y.dtCreation));
                //TODO Upgrade this horrible way to fix a loop serialization
                mensagens.ForEach(m => m.SetUser());
                /*
                 * NOTE: 
                 * - Find way to compress or reduce the string
                 * Temporal fix:
                 * - Only load last 5 messages
                 * */
                //
                return mensagens;
            } 
            
        }

        private void saveMessage(string msg)
        {
            try
            {
                using (ChatBDContainer chatBDContainer = new ChatBDContainer())
                {
                    //Instancia uma nova mensagem
                    Mensagens new_mensagen = new Mensagens();
                    //Message Owner
                    new_mensagen.IdUser = client.IdUser;
                    //Split message
                    new_mensagen.key = Convert.FromBase64String(msg.Split('$')[0]);
                    new_mensagen.iv = Convert.FromBase64String(msg.Split('$')[1]);
                    new_mensagen.Text = Convert.FromBase64String(msg.Split('$')[2]);
                    //Save message
                    chatBDContainer.MensagensSet.Add(new_mensagen);
                    //Guarda as alterações efetuadas
                    chatBDContainer.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogController logController = new LogController();
                logController.consoleLog(ex.Message, "Server");
            }
        }

        // Envia mensagem a todas a ligações
        private void broadCast(byte[] data)
        {
            logger.consoleLog("Sending message", this.client.Username);
            // Loop por cada cliente
            foreach (KeyValuePair<Users, TcpClient> client in ClientsDictionary) 
            {
                // Envia a mensagem ao cliente
                try
                {
                    client.Value.GetStream().Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    logger.consoleLog("Error sending message to " + client.Key.Username, this.client.Username);
                }
            }
        }

    }

}

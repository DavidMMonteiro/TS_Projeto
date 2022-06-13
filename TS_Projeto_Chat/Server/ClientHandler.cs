using EI.SI;
using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

/*
 * Esta classe tem a função para criar as novas Threads e gerir para os clientes
 * Para esta classe é precisso:
 * - LogController: class utilizada para controlar os logs do servidor
 * - TcpClient: stream do cliente
 * - User: Utilizador do cliente
 * - ClientsDictionary: Dicionario com todos os clientes e ligação dos clientes ativas no servidor 
 */

namespace TS_Chat
{
    // Thread para do client
    public class ClientHandler
    {
        private LogController logger;
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
            logger = new LogController(this.client.Username);
            logger.consoleLog(msg);
            byte[] ack = protocol.Make(ProtocolSICmdType.DATA, cryptor.SingData(msg));
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
                        // Utilizada para gerir mensagens normais transmidos pelos clientes
                        case ProtocolSICmdType.DATA:
                            //Recebe a data do cliente
                            output = protocolSI.GetStringFromData();
                            //Envia o log ao servidor
                            logger.consoleLog("Saving message", this.client.Username);
                            //Guarda a mensagem na base de dados
                            saveMessage(output);
                            //Constroe a mensagem
                            ack = protocolSI.Make(ProtocolSICmdType.DATA, output);
                            //Envia a mensagem
                            broadCast(ack);
                            break;
                        // Tipo EOT utilizada para fechar a ligação com o servidor
                        case ProtocolSICmdType.EOT:
                            //Constroe a mensagem para o cliente
                            output = this.client.Username + " left the chat";
                            //Envia o log ao servidor
                            logger.consoleLog(output);
                            //Encrypta os dados e faz o sing do dados
                            output = cryptor.SingData(output);
                            //Prepara a mensagem
                            ack = protocolSI.Make(ProtocolSICmdType.EOT, output);
                            //Envia a mensagem
                            broadCast(ack);
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            //Envia o log ao servidor
                            logger.consoleLog("Sending chat to request", "Server");
                            //Carrega as mensagens da base de dados
                            List<Mensagens> chats = LoadChat();
                            //Valida que a mensagens
                            if (chats.Count > 0)
                            {
                                //Envia o log ao servidor
                                logger.consoleLog("Serializing chat message to JSON", "Server");
                                //Loop por cada mensagem
                                foreach (Mensagens chat in chats) 
                                {
                                    try
                                    {
                                        //Transgorma a mensagem em string de estrutura de JSON
                                        string msg = JsonConvert.SerializeObject(chat);
                                        //Encrypta a mensagem e faz sing os dados
                                        msg = cryptor.SingData(msg);
                                        //Prepara a mensagem
                                        ack = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, msg);
                                        //Envia a mensagem
                                        unicast(ack);
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
                    //Caso erro com o socket
                    error = $"(Server): Error processing message.\n - Socket error\nGet in contact with administration";
                    logger.consoleLog(ex.Message, this.client.Username);

                }
                catch (IOException ex) // Excepção de Socket
                {
                    //Erro com os objetos
                    error = $"(Server): Error processing message.\n - IOException\nGet in contact with administration";
                    logger.consoleLog("Socket error: " + ex.Message, this.client.Username);
                }
                catch (Exception ex) // Excepção desconhecida 
                {
                    //Erro unknow
                    error = $"(Server): Error processing message.\n - Unknow error catch\nGet in contact with administration";
                    logger.consoleLog("Uncommon error: " + ex.Message, this.client.Username);
                }
                
                //Caso encontrar algum erro, envia a mensagem ao cliente para fechar o a ligação
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

        //Carrega as mensagens da base de dados
        private static List<Mensagens> LoadChat()
        {
            //Initialize logger
            LogController logger = new LogController();
            //Envia o log ao servidor
            logger.consoleLog("Loading chat", "Server");
            using (ChatBDContainer chatBDContainer = new ChatBDContainer())
            {
                //Envia o log ao servidor
                logger.consoleLog("Sorting chat by date time", "Server");
                //Carrega as mensagens da base de dados
                List<Mensagens> mensagens = chatBDContainer.MensagensSet.ToList();
                //Faz ordem das mensagens por data
                mensagens.Sort((x, y) => x.dtCreation.CompareTo(y.dtCreation));
                //Especifica a informação dos utilizadores
                mensagens.ForEach(m => m.SetUser());
                /*
                 * NOTE: 
                 * - Find way to compress or reduce the string to less 1400
                 * */
                //
                return mensagens;
            } 
            
        }

        //Guarda a mensagem na base de dados
        //NOTE: Esta funcional, guardado todas as mensagens enviadas ao servidor
        private void saveMessage(string msg)
        {
            try
            {
                using (ChatBDContainer chatBDContainer = new ChatBDContainer())
                {
                    
                    //Initialize Decryptor
                    Cryptor cryptor = new Cryptor();
                    //Get info message
                    Mensagens new_mensagen = cryptor.GetVerifyMessage(msg);
                    //Validate new message
                    if (new_mensagen == null)
                        return;
                    //Message Owner
                    new_mensagen.IdUser = client.IdUser;
                    //Save message
                    chatBDContainer.MensagensSet.Add(new_mensagen);
                    //Guarda as alterações efetuadas
                    chatBDContainer.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogController logController = new LogController();
                //Envia o log ao servidor
                logController.consoleLog(ex.Message, "Server");
            }
        }

        // Envia mensagem a todas a ligações
        private void broadCast(byte[] data)
        {
            logger.consoleLog("Sending broadcast message", this.client.Username);
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
                    logger.consoleLog("Error sending broadcast message to " + client.Key.Username, this.client.Username);
                }
            }
        }

        //Envia a mensagem a proprio cliente
        private void unicast(byte[] data)
        {
            logger.consoleLog("Sending unicast message", this.client.Username);
            // Loop por cada cliente
            TcpClient client = this.ClientsDictionary.First(c => c.Key.IdUser == this.client.IdUser).Value;
            // Envia a mensagem ao cliente
            try
            {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                logger.consoleLog("Error sending unicast message to " + this.client.Username, this.client.Username);
            }
        }
    }
}

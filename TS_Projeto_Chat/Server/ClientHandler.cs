using EI.SI;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TS_Chat
{
    //TODO Make enum generic
    public enum TransmisionType{
        unicast, 
        multicast, 
        broadcast
    }
    
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
            string msg = $"{this.client.Username} join the chat";
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
            NetworkStream networkStream = this.tcpClient.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            // Enquando a mensagem do cliente não for EOT
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    // Vai ler a mensagem do cliente
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    string output;
                    int chatDestiny;
                    string msg;
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
                            logger.consoleLog(output, this.client.Username);
                            ack = protocolSI.Make(ProtocolSICmdType.DATA, $"({this.client.Username}): {output}");
                            broadCast(ack);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = this.client.Username + " left the chat";
                            logger.consoleLog(output);
                            ack = protocolSI.Make(ProtocolSICmdType.EOT, output);
                            //Make Switch kind of TransmisionType
                            //Get chat from database
                            //
                            if (true /*Chat id = 1*/)
                                broadCast(ack);
                            else if (true/*Chat have multiple destinations*/)
                                multiCast(ack, new List<int>());
                            else
                                unicast(ack, 0);
                            break;
                    }
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
                        break;
                }
                catch (SocketException ex)
                {
                    logger.consoleLog(ex.Message, this.client.Username);
                    break;
                }
                catch (IOException ex) // Excepção de Socket
                {
                    logger.consoleLog("Socket error: " + ex.Message, this.client.Username);
                    break;
                }
                catch (Exception ex) // Excepção desconhecida 
                {
                    logger.consoleLog("Uncommon error: " + ex.Message, this.client.Username);
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

        // Envia mensagem a todas a ligações
        private void broadCast(byte[] data)
        {
            // Loop por cada cliente
            foreach (KeyValuePair<Users, TcpClient> client in ClientsDictionary)
                // Envia a mensagem ao cliente
                client.Value.GetStream().Write(data, 0, data.Length);
        }

        // Envia mensagem a uma lista de clientes
        private void multiCast(byte[] data, List<int> destenies)
        {
            foreach (int destiny in destenies)
            {
                Users find_user = new Users();
                find_user.IdUser = destiny;
            }
                /*TcpClient tcpClient = this.ClientsDictionary.ContainsKey(destiny) ? ClientsDictionary[destiny] : null;
                    client.GetStream().Write(data, 0, data.Length);*/
        }

        // Envia mensagem a so um client
        private void unicast(byte[] data, int desteny)
        {
            // Valida se o destino existe no dictionario
           /* if (ClientsDictionary.TryGetValue(desteny, out TcpClient client))
                // Envia ao destino se existir
                client.GetStream().Write(data, 0, data.Length);*/
        }
        
    }

}

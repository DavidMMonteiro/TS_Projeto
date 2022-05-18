using EI.SI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TS_Chat
{
    // Thread para do client
    public class ClientHandler
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
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
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
            foreach (KeyValuePair<string, TcpClient> client in ClientsDictionary)
                // Envia a mensagem ao cliente
                client.Value.GetStream().Write(data, 0, data.Length);
        }

        // Envia mensagem a uma lista de clientes
        private void multiCast(byte[] data, List<string> destenies)
        {
            // Itenera pelos nomes destino
            foreach (string desteny in destenies)
                // Valida se o nome destino existe no dicionario
                if (ClientsDictionary.TryGetValue(desteny, out TcpClient client))
                    // Envia a mensagem ao cliente especifico
                    client.GetStream().Write(data, 0, data.Length);
        }

        // Envia mensagem a so um client
        private void unicast(byte[] data, string desteny)
        {
            // Valida se o destino existe no dictionario
            if (ClientsDictionary.TryGetValue(desteny, out TcpClient client))
                // Envia ao destino se existir
                client.GetStream().Write(data, 0, data.Length);
        }
    }

}

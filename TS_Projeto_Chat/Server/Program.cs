using System;
using System.Net;
using System.Net.Sockets;
using EI.SI;
using System.Threading;

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
                    //Console info
                    helper.consoleLog(dataFromClient.Split('$')[0] + " connected", name);
                    //Create Cliente Handler
                    ClientHandler clientHandler = new ClientHandler(client, dataFromClient.Split('$')[0]);
                    clientHandler.Handle();
                }catch (Exception ex){
                    helper.consoleLog(ex.Message, name);
                    break;
                }
                
            }
        
        }
            
    } 
    
    class ClientHandler
    {
        Helper helper = new Helper();
        private TcpClient client;
        private string clientName;

        public ClientHandler(TcpClient client, string clientName)
        {
            this.client = client;
            this.clientName = clientName;
        }


        public void Handle()
        {
            Thread thread = new Thread(threadHandler);
            thread.Start();
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
                            helper.consoleLog(output.Split('$')[1], this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                        case ProtocolSICmdType.EOT:
                            output = "Ending Threading from " + this.clientName;
                            helper.consoleLog(output.Split('$')[1], this.clientName);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
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
                catch (System.IO.IOException ex)
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
    }

    class Helper
    {
        public void consoleLog(string msg, string owner)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + owner + ": " + msg);
        }
    }
}


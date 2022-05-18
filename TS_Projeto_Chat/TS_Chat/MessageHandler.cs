using EI.SI;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TS_Chat
{
    // Clase para controlar a leitura de mensagens
    class MessageHandler
    {
        private TcpClient client;
        private ChatController chatController;
        private Thread messageThread;

        // MessageHandler constructor
        public MessageHandler(TcpClient client, ChatController chatController)
        {
            this.client = client;
            this.chatController = chatController;
            this.messageThread = Handle();
        }

        // Handler para iniciar a nova Thread
        public Thread Handle()
        {
            Thread thread = new Thread(threadHandler);
            thread.Start();
            return thread;
        }

        // Função que vai desenvolver a thread
        private void threadHandler()
        {
            // Guarda a networkStream no cliente
            NetworkStream networkStream = this.client.GetStream();

            ProtocolSI protocolSI = new ProtocolSI();
            // Loop ate receber mensagem do servidor a fechar o ligação
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    // Lee a mensagem envia pelo servidor
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    string output;
                    // Filtra o tipo de mensagem
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            // Lee a mensagem 
                            output = protocolSI.GetStringFromData();
                            // Escreve a mensagem para o cliente
                            chatController.newMessage(output);
                            break;
                        case ProtocolSICmdType.EOT:
                            // Lee a mensagem 
                            output = protocolSI.GetStringFromData();
                            // Escreve a mensagem para o cliente
                            chatController.newMessage(output);
                            break;
                    }
                }// Change Exception to show on Console on last version
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message, "Error SocketExcpetion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error IOException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (ObjectDisposedException ex)
                {
                    MessageBox.Show(ex.Message, "Error ObjectDisposed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                /*catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unknow Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }*/
            }
            // Fecha a ligação
            networkStream.Close();
            client.Close();
        }
    }
}

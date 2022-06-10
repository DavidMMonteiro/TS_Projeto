using EI.SI;
using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    string output = protocolSI.GetStringFromData();
                    //Get Key and IV decryptor
                    string key = output.Split('$')[0];
                    string iv = output.Split('$')[1];
                    //Initialize Decryptor
                    Cryptor cryptor = new Cryptor();
                    //Decrypt message
                    string message = cryptor.DesencryptText(key, iv, output.Split('$')[2]);
                    //Get the message owner
                    string owner = output.Split('$')[3];
                    // Filtra o tipo de mensagem
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            // Escreve a mensagem para o cliente
                            chatController.newMessage(owner,message);
                            break;
                        case ProtocolSICmdType.EOT:
                            // Escreve a mensagem para o cliente
                            chatController.newMessage(message);
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            LoadChat(message);
                            break;
                        case ProtocolSICmdType.USER_OPTION_9:
                            output = protocolSI.GetStringFromData();
                            chatController.consoleLog(output);
                            break;
                    }
                }// Change Exception to show on Console on last version
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message + '\n' + ex.ToString(), "Error SocketExcpetion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message + '\n' + ex.ToString(), "Error IOException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (ObjectDisposedException ex)
                {
                    MessageBox.Show(ex.Message + '\n' + ex.ToString(), "Error ObjectDisposed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + '\n' + ex.ToString(), "Unknow Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            // Fecha a ligação
            networkStream.Close();
            client.Close();
        }

        private void LoadChat(string output)
        {
            List<Mensagens> mensagens = JsonConvert.DeserializeObject<List<Mensagens>>(output);
            foreach(Mensagens mensangem in mensagens)
                chatController.newMessage(mensangem.Users.Username, mensangem.Text);
        }
    }
}

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
        public bool active;

        // MessageHandler constructor
        public MessageHandler(TcpClient client, ChatController chatController)
        {
            this.client = client;
            this.chatController = chatController;
            this.messageThread = Handle();
            this.active = true;
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
            //
            Mensagens old_mensagem = new Mensagens();
            // Loop ate receber mensagem do servidor a fechar o ligação
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    if (networkStream.CanRead == null)
                        return;
                    // Lee a mensagem envia pelo servidor
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    //Get message
                    string output = protocolSI.GetStringFromData();
                    //Initialize Decryptor
                    Cryptor cryptor = new Cryptor();
                    //Decrypt message
                    string message = cryptor.VerifyData(output);
                    // Filtra o tipo de mensagem
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            //Check if message it's subencrypted
                            if(message.Split('$').Length > 1)
                                message = cryptor.DesencryptText(message);
                            //Validate if it's a server message 
                            if(message.Split('$').Length == 1)
                                chatController.newMessage(message);
                            //Validate if it's a client message
                            else if (message.Split('$').Length == 2)
                                chatController.newMessage(message.Split('$')[1], message.Split('$')[0]);
                            break;
                        case ProtocolSICmdType.EOT:
                            // Escreve a mensagem para o cliente
                            chatController.newMessage(message);
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            try
                            {                                
                                Mensagens new_mensagem = JsonConvert.DeserializeObject<Mensagens>(cryptor.VerifyData(output));
                                if (old_mensagem.IdMensagem != new_mensagem.IdMensagem)
                                {
                                    old_mensagem = new_mensagem;
                                    chatController.newMessage(new_mensagem.GetMessage().Split('$')[1], new_mensagem.GetMessage().Split('$')[0]);
                                }
                            }catch (Exception ex)
                            {
                                MessageBox.Show("Loading messages error: \n" + ex.Message + "\nIf keeps happening contact Administrator", "Unknow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case ProtocolSICmdType.USER_OPTION_9:
                            output = protocolSI.GetStringFromData();
                            chatController.consoleLog(output);
                            break;
                    }
                }// Change Exception to show on Console on last version
                catch (SocketException ex)
                {
                    MessageBox.Show("Error connecting with server: \n" + ex.Message + "\nContact Administrator", "Error SocketExcpetion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Error connecting with server: \n" + ex.Message + "\nContact Administrator", "Error IOException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (ObjectDisposedException ex)
                {
                    MessageBox.Show("Error connecting with server: \n" + ex.Message + "\nContact Administrator", "Error ObjectDisposed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting with server: \n" + ex.Message + "\nContact Administrator", "Unknow Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            // Fecha a ligação
            networkStream.Close();
            client.Close();
            this.active = false;
        }

    }
}

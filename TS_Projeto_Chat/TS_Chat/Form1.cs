using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using EI.SI;

namespace TS_Projeto_Chat
{
    public partial class Form1 : Form
    {
        // Atributos para conneção com  o Servidor 
        private int port;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private TcpClient client;
        private string name;
        private ChatController chatController;
        private MessageHandler messageHandler;

    	/* 
        Quando e construido o form, recebe a informação da ligaçã establecida 
        com o servidor no form anterior
        */ 
        public Form1(int port, NetworkStream network, ProtocolSI protocol, TcpClient client, string name)
        {
            InitializeComponent();
            this.port = port;
            this.networkStream = network;
            this.protocolSI = protocol;
            this.client = client;
            this.name = name;
            this.Text = "Chatting as: " + name;
            lb_chat.Text = name;
            this.chatController = new ChatController(tb_chat);
            this.messageHandler = new MessageHandler(this.client, this.chatController);
        }

        // Fecha o ligação do cliente com o servidor-
        private void CloseClient()
        {
            try
            {
                //Constroe uma mensagem para o servidor para fechar o canal
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                //Escreve a mensagem ao servidor
                networkStream.Write(eot, 0, eot.Length);
                // Espera pela resposta do servidor
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                // Fecha o canal
                networkStream.Close();
                chatController.newMessage(this.name, "Fechar client... bye :-)");
                client.Close();
            }
            catch (Exception ex)
            {
                string msg = "(error): " + ex.Message;
                chatController.consoleLog(msg);
            }
        }

        // Establece a ligação com o servidor
        private void connect_server()
        {
            try
            {
                // Constreo a mensagem para ligar ao servidor
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, this.name + "$");
                // Envia a mensagem ao servidor
                networkStream.Write(packet, 0, packet.Length);
                chatController.newMessage(this.name, "Connected to server");
                bt_send.Enabled = false;
            }
            catch (Exception ex)
            {
				// Caso a ligação fallar, informa ao utilizador
                chatController.newMessage(this.name, "Connection to server fail... Try later...");
                bt_send.Enabled = true;
            }
        }

        // Envia a mensagem ao servidor
        private void send_message()
        {
            // Valida que existe uma mensagem
            if (tb_message.Text == "")
                return;
            string msg = tb_message.Text;
            try
            {
                // Preparar mensagem para o servidor
                //newMessage(this.name, tb_message.Text);
                tb_message.Clear();
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                // Manda a mensagem ao servidor
                networkStream.Write(packet, 0, packet.Length);
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            }
            catch (Exception ex)
            {
				// Caso a mensagem sofrer erro ao ser enviada
                chatController.newMessage(this.name, "Erro ao comunicar com o servidor.\r\n" + ex.Message);
                bt_send.Enabled = false;
            }
        }

        // Establece ligação com o servidor
        private void bt_connect_Click(object sender, EventArgs e)
        {
            connect_server();
        }

        // Quando o butão e clickado envia a mensagem ao servidor
        private void bt_send_Click(object sender, EventArgs e)
        {
            send_message();
        }

        // Quando uma tecla e clickada envia a mensagem ao servidor
        private void tb_message_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Valida a tecla a ser clickada, neste caso enter
            if (e.KeyChar == (char)Keys.Enter)    
                send_message();
        }

        // Quando o form é fechado, fecha o canal do cliente com o servidor
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
            // Fecha a aplicação por completo
            Application.Exit();
        }

        // Efetua o logout do cliente, fechado a ligação já establecido 
        // e abre o form de login para o utilizador 
        private void bt_logout_Click(object sender, EventArgs e)
        {
            this.messageHandler.CloseThread();
            CloseClient();
            Form_Login form_login = new Form_Login();
            form_login.Show();
            this.Hide();
        }
    }

    class ChatController
    {
        private TextBox textBox;

        public ChatController(TextBox textBox)
        {
            this.textBox = textBox;
        }
        public void newMessage(string msg)
        {
            consoleLog(msg);
            if (textBox.InvokeRequired)          
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText($"\r\n{msg}"); });
            else           
                textBox.AppendText($"\r\n{msg}");
        }
        public void newMessage(string owner, string msg)
        {
            string data = $"({owner}): {msg}";
            consoleLog(data);
            if (textBox.InvokeRequired)
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText("\r\n"+data); });
            else
                textBox.AppendText("\r\n"+data);

        }
        public void consoleLog(string msg)
        {
            msg = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + msg;
            logFile(msg);
            Console.WriteLine(msg);
        }

        private void logFile(string msg)
        {
            try { 
                string pathFile = "chat_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
                if (!File.Exists(pathFile))
                    File.Create(pathFile);

                File.AppendAllText(pathFile, "\r\n" + msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                
        }


    }
    class MessageHandler
    {
        private TcpClient client;
        private ChatController chatController;
        private Thread messageThread;

        public MessageHandler(TcpClient client, ChatController chatController)
        {
            this.client = client;
            this.chatController = chatController;
            this.messageThread = Handle();
        }


        public Thread Handle()
        {
            Thread thread = new Thread(threadHandler);
            thread.Start();
            return thread;
        }

        public void CloseThread()
        {
            this.messageThread.Interrupt();
        }

        private void threadHandler()
        {
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            //
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    string output;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            output = protocolSI.GetStringFromData();
                            chatController.newMessage(output);
                            break;
                    }
                }
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
                catch (ObjectDisposedException)
                { 
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unknow Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }

            networkStream.Close();
            client.Close();
        }
    }
}
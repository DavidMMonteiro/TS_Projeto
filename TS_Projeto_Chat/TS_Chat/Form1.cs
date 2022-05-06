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
        private int port;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private TcpClient client;
        private string name;
        private ChatController chatController;

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
            ServerHandler server = new ServerHandler(this.client, chatController);
        }

        private void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + this.name + ": " + msg);
        }

        private void CloseClient()
        {
            try
            {
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                networkStream.Close();
                chatController.newMessage(this.name, "Fechar client... bye :-)");
                client.Close();
            }
            catch (Exception ex)
            {
                consoleLog(ex.Message);
                //newMessage(this.name , "Error ao sair do servidor\r\n\t" + ex.Message);
            }
        }

        private void connect_server()
        {
            try
            {
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, this.name + "$");
                networkStream.Write(packet, 0, packet.Length);
                chatController.newMessage(this.name, "Connected to server");
                bt_send.Enabled = false;
            }
            catch (Exception ex)
            {
                chatController.newMessage(this.name, "Connection to server fail... Try later...");
                consoleLog(ex.Message);
                bt_send.Enabled = true;
            }
        }
        private void send_message()
        {
            if (tb_message.Text == "")
                return;
            string msg = tb_message.Text;
            try
            {
                // Preparar mensagem para o servidor
                //newMessage(this.name, tb_message.Text);
                tb_message.Clear();
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            }
            catch (Exception ex)
            {
                chatController.newMessage(this.name, "Erro ao comunicar com o servidor.\r\n" + ex.Message);
                bt_send.Enabled = false;
            }
        }
        private void bt_connect_Click(object sender, EventArgs e)
        {
            connect_server();
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            send_message();
        }

        private void tb_message_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                send_message();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
            Application.Exit();
        }

        private void bt_logout_Click(object sender, EventArgs e)
        {
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
            if (textBox.InvokeRequired)          
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText($"\r\n{msg}"); });
            else           
                textBox.AppendText($"\r\n{msg}");
        }
        public void newMessage(string owner, string msg)
        {
            if (textBox.InvokeRequired)
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText($"\r\n({owner}): {msg}"); });
            else
                textBox.AppendText($"\r\n({owner}): {msg}");

        }


    }
    class ServerHandler
    {
        private TcpClient client;
        private ChatController chatController;

        public ServerHandler(TcpClient client, ChatController chatController)
        {
            this.client = client;
            this.chatController = chatController;
            Handle();
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
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                try
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    string output;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.ACK:
                            output = protocolSI.GetStringFromData();
                            chatController.newMessage(output);
                            break;
                        default:
                            output = "Protocol Type not know";
                            MessageBox.Show("Erro Desconhecido", output, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show(ex.Message, "Error IOException",MessageBoxButtons.OK, MessageBoxIcon.Error);
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
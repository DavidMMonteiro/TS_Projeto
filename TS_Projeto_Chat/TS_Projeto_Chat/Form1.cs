using System.Net.Sockets;
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

        public Form1(int port, NetworkStream network, ProtocolSI protocol, TcpClient client, string name)
        {
            InitializeComponent();
            this.port = port;
            this.networkStream = network;
            this.protocolSI = protocol;
            this.client = client;  
            this.name = name;
            lb_chat.Text = name;
        }

        private void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + this.name +": " + msg);
        }

        private void newMessage(string owner, string msg)
        {
            tb_chat.AppendText("\r\n(" + owner + "): " + msg);
            consoleLog(msg);
        }

        private void CloseClient()
        {
            try
            {
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                networkStream.Close();
                newMessage(this.name, "Fechar client... bye :-)");
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
                newMessage(this.name , "Connected to server");
                bt_send.Enabled = true;
            }
            catch (Exception ex)
            {
                newMessage(this.name , "Connection to server fail... Try later...");
                consoleLog(ex.Message);
                bt_send.Enabled = false;
            }
        }
        private void send_message()
        {
            string msg = tb_message.Text;
            try
            {
                // Preparar mensagem para o servidor
                newMessage(this.name, tb_message.Text);
                tb_message.Clear();
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            }
            catch (Exception ex)
            {
                newMessage(this.name, "Erro ao comunicar com o servidor.\r\n" + ex.Message);
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
}
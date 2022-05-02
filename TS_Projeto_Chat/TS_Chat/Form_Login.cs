using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TS_Projeto_Chat
{
    public partial class Form_Login : Form
    {
        private const int port = 10000;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private TcpClient client;
        public Form_Login()
        {
            InitializeComponent();
        }

        private void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + "Login: " + msg);
        }
        
        private bool open_connection()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                client = new TcpClient();
                client.Connect(endPoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool login_Server(string username, string password)
        {
            if(!open_connection())
                return false;
            try
            {
                
                string msg = username + "$" + password;
                protocolSI = new ProtocolSI();
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                // Espera informação do servidor
                do { 
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                } while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK);
                    
                string serermsg = protocolSI.GetStringFromData();
                return bool.Parse(serermsg);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Login fail", "Check user name or Password");
                MessageBox.Show("Connection fail", "Connection to server fail... Try later...");
                consoleLog(ex.Message);
                return false;
            }

        }
        private void bt_login_Click(object sender, EventArgs e)
        {
            if (!login_Server(tb_user.Text, tb_password.Text)) {
                tb_password.Text = null;
                return; 
            }

            Form1 chat = new Form1(port, networkStream, protocolSI, client, tb_user.Text);
            chat.Show();
            this.Hide();
        }

        private void Form_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}

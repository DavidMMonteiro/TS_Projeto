using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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

        private bool login_Server(string username, string password)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                client = new TcpClient();
                client.Connect(endPoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();
                string msg = username + "$" + password;
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection to server fail... Try later...");
                return false;
            }

        }
        private void bt_login_Click(object sender, EventArgs e)
        {
            if (!login_Server(tb_user.Text, tb_password.Text))
                return;

            Form1 chat = new Form1(port, networkStream, protocolSI, client, tb_user.Text);
            chat.Show();
            this.Hide();

        }
    }
}

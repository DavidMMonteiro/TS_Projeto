using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using TS_Chat;

namespace TS_Projeto_Chat
{
    public partial class Form_Login : Form
    {
        // Atributos para conneção com  o Servidor 
        private const int PORT = 10000;
        private NetworkStream NetworkStream;
        private ProtocolSI ProtocolSI;
        private TcpClient Client;
        private string PrivateKey;
        private string Vetor;

        public Form_Login()
        {
            InitializeComponent();
        }

        // Escreve na consola a mensagem recebida com a data de emissão
        private void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + "Login: " + msg);
        }
        
        /* 
        Abre uma ligação com o servidor
        Caso não conseguir establecer a ligação retorna false, caso contrario true
        */
        private bool open_connection()
        {
            try
            {
                // Cria a localização do servidor (IP local e porta)
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                // Inicializa o TCPclientes
                Client = new TcpClient();
                // Vas a ligação com o servição
                Client.Connect(endPoint);
                // Abre a ligação com o servidor
                NetworkStream = Client.GetStream();
                // Inicializa a class ProtocolISI
                ProtocolSI = new ProtocolSI();
                return true;
            }
            catch (Exception ex)
            {
                // Mostra a mensagem de erro ao cliente
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /*
        Faz a comunicação do cliente com o servidor para efetuar o login do cliente
        Caso não conseguir efetuar o login retorna false, caso contrario true.
        */
        private bool login_Server(string username, string password)
        {
            // Tenta establecer ligação com o servidor
            if(!open_connection())
                return false;

            try
            {
                //Constroe a mensagem do cliente
                //TODO  Encrypte password & message to server
                string msg = username + "$" + password;
                ProtocolSI = new ProtocolSI();
                // Converte a mensagem para bytes para poder ser enviada
                byte[] packet = ProtocolSI.Make(ProtocolSICmdType.ACK, msg);
                // Faz a transmição da mensagem
                NetworkStream.Write(packet, 0, packet.Length);
                // Espera pela informação do servidor
                do { 
                    NetworkStream.Read(ProtocolSI.Buffer, 0, ProtocolSI.Buffer.Length);
                } while (ProtocolSI.GetCmdType() != ProtocolSICmdType.ACK);
                // Lee a informação da mensagem returnada pelo servidor
                string serermsg = ProtocolSI.GetStringFromData();
                bool loginState = bool.Parse(serermsg.Split('$')[0]);
                this.PrivateKey = serermsg.Split('$')[1];
                this.Vetor = serermsg.Split('$')[2];
                // Converte para bool
                return loginState;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection fail", "Connection to server fail... Try later...");
                consoleLog(ex.Message);
                return false;
            }

        }

        // Acção do butão login
        private void bt_login_Click(object sender, EventArgs e)
        {
            //Validação que os dados foram inseridos
            if (string.IsNullOrEmpty(tb_user.Text))
            {
                MessageBox.Show("Preencha o cambo do username!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(tb_password.Text))
            {
                MessageBox.Show("Preencha o cambo da password!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Valida os dados do cliente e se consegue efetuar o login com po servidor
            if (!login_Server(tb_user.Text, tb_password.Text)) {
                tb_password.Text = null;
                MessageBox.Show("Erro no login...\nExpirimente novamente!", "Login fail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }

            // Se for bem sucedido, abré os chats ao cliente
            Form1 chat = new Form1(PORT, NetworkStream, ProtocolSI, Client, tb_user.Text);
            chat.Show();
            this.Hide();
        }

        private void bt_singup_Click(object sender, EventArgs e)
        {
            new FormSignUp(this, PORT, NetworkStream, ProtocolSI, Client).ShowDialog();
        }

        private void Form_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}

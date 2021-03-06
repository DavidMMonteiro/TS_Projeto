using EI.SI;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TS_Chat
{
    public partial class FormSignUp : Form
    {
        private Form FormBack; //Form para voltar
        private int port; //Port de ligação com o servidor
        private NetworkStream networkStream; //Stream de ligação
        private ProtocolSI protocolSI; //
        private TcpClient client; //Ligação establecida com o cliente

        //Recebe os dados do login
        public FormSignUp(Form formBack, int port, NetworkStream networkStream, ProtocolSI protocolSI, TcpClient client)
        {
            InitializeComponent();
            this.FormBack = formBack;
            this.FormBack.Hide();
            this.port = port;
            this.networkStream = networkStream;
            this.protocolSI = protocolSI;
            this.client = client;
        }
        //Establece a ligação com o servidor
        private bool open_connection()
        {
            try
            {
                // Cria a localização do servidor (IP local e porta)
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                // Inicializa o TCPclientes
                client = new TcpClient();
                // Vas a ligação com o servição
                client.Connect(endPoint);
                // Abre a ligação com o servidor
                networkStream = client.GetStream();
                // Inicializa a class ProtocolISI
                protocolSI = new ProtocolSI();
                return true;
            }
            catch (Exception ex)
            {
                // Mostra a mensagem de erro ao cliente
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //Evento para sair do form Registo
        private void bt_cancel_Click(object sender, EventArgs e)
        {
            //Valida se o utilizador quere sair
            if (MessageBox.Show("Quere sair do SigUp?", "Exit Signup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.FormBack.Show();
                this.Close();
            }

        }

        //Evento para efetura o registo do utilizador
        private void bt_registar_Click(object sender, EventArgs e)
        {
            //Valida o username prenchida 
            if (string.IsNullOrEmpty(tb_username.Text))
            {
                MessageBox.Show("Insira os dados no cambo username", "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Valida a password prenchida
            else if (string.IsNullOrEmpty(tb_password.Text) || string.IsNullOrEmpty(tb_re_password.Text))
            {
                MessageBox.Show("Insira os dados nos campos de password", "Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Valida que as password iguais
            else if (tb_password.Text != tb_re_password.Text)
            {
                MessageBox.Show("As password tem de ser iguais!","Sign Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tb_password.Text = "";
                tb_re_password.Text = "";
                return;
            }

            //Estable a ligação
            if (!open_connection())
                return;

            try
            {
                Cryptor cryptor = new Cryptor();
                //Constroe a mensagem do cliente
                string msg = tb_username.Text + "$" + tb_password.Text;
                //Encripta a mensagem
                msg = cryptor.SingData(msg);
                //
                protocolSI = new ProtocolSI();
                // Converte a mensagem para bytes para poder ser enviada
                byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, msg);
                // Faz a transmição da mensagem
                networkStream.Write(packet, 0, packet.Length);
                // Espera pela informação do servidor
                do
                {
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                } while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK);
                // Lee a informação da mensagem returnada pelo servidor
                string servermsg = cryptor.VerifyData(protocolSI.GetStringFromData());
                //Lee a primeira parte da mensagem, sendo o resultado o registo
                bool signUp = bool.Parse(servermsg.Split('$')[0]);
                //Lee a 2º parte da mensagem, contendo esta o erro de parte do servidor
                string errorText = servermsg.Split('$')[1];
                //Valida que o registo foi criado
                if (signUp)
                {
                    //Output to user
                    MessageBox.Show("Conta criada com sucesso!", "Criar conta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Return to login form
                    this.FormBack.Show();
                    this.Close();
                }
                else
                    MessageBox.Show(errorText, "Criar conta", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado ao criar a conta!", "Criar conta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}

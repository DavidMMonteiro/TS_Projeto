using System.Net;
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

        enum MessageType{
            Connection = 0,
            Login = 1,
            Message = 2,
            Error = 3
        }

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
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + name +": " + msg);
        }

        private void newMessage(string msg)
        {
            string owner = msg.Split("$")[0];
            string text = msg.Split("$")[1];
            tb_chat.AppendText("\r\n(" + owner + "): " + text);
            consoleLog(text);
        }

        private void CloseClient()
        {
            try
            {
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                networkStream.Close();
                string msg = this.name + "$" + "Fechar client... bye :-)";
                newMessage(msg);
                client.Close();
            }
            catch (Exception ex)
            {
                newMessage(this.name + "$" + "Error ao sair do servidor\r\n\t" + ex.Message);
            }
        }

        private void bt_connect_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                client = new TcpClient();
                client.Connect(endPoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, name + "$");
                networkStream.Write(packet, 0, packet.Length);
                newMessage(this.name + "$" + "Connected to server");
                bt_send.Enabled = true;
            }
            catch (Exception ex)
            {
                newMessage(this.name + "$" + "Connection to server fail... Try later...");
                bt_send.Enabled = false;
            }
            
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            // Leer mensagem
            string msg = name + "$" + tb_message.Text;
            tb_message.Clear();
            try
            {
                // Preparar mensagem para o servidor
                newMessage(msg);
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                consoleLog("Mensagem enviada ao servidor...");
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)                
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                
            }
            catch (Exception ex)
            {
                newMessage(this.name + "$" + "Erro ao comunicar com o servidor.\r\n" + ex.Message);
                bt_send.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
        }
    }
}
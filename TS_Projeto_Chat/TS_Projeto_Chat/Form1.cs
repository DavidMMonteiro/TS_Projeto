using System.Net;
using System.Net.Sockets;
using EI.SI;

namespace TS_Projeto_Chat
{
    public partial class Form1 : Form
    {
        private const int port = 10000;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private TcpClient client;
        private string name;
        public Form1()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
            client = new TcpClient();
            client.Connect(endPoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();
            name = lb_chat.Text;
        }

        private void consoleLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("(dd/MM/yyyy HH:mm:ss)") + name +": " + msg);
        }

        private void newMessage(string msg)
        {
            string owner = msg.Split("$")[0];
            tb_chat.AppendText("\r\n(" + owner + "): " + msg);
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            // Leer mensagem
            string msg = name + "$" + tb_message.Text;
            tb_message.Clear();
            try
            {
                // Preparar mensagem para o servidor
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
                networkStream.Write(packet, 0, packet.Length);
                consoleLog("Mensagem enviada ao servidor...");
                // Espera informação do servidor
                while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                {
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    newMessage(msg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao comunicar com o servidor");
                MessageBox.Show(ex.Message);
            }
        }
    }
}
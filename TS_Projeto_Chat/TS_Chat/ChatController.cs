using System.Windows.Forms;

namespace TS_Chat
{
    // Class que controla a caixa das mensagem
    class ChatController : LogController
    {
        private TextBox textBox;

        //ChatController constructor
        public ChatController(TextBox textBox)
        {
            this.textBox = textBox;
        }

        //Escreve nova mensagem simples
        public void newMessage(string msg)
        {
            consoleLog(msg);
            if (textBox.InvokeRequired)
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText($"\r\n{msg}"); });
            else
                textBox.AppendText($"\r\n{msg}");
        }

        //Escreve nova mensagem composta 
        public void newMessage(string owner, string msg)
        {
            consoleLog(msg, owner);
            string data = $"({owner}): {msg}";
            if (textBox.InvokeRequired)
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText("\r\n" + data); });
            else
                textBox.AppendText("\r\n" + data);

        }

    }
}

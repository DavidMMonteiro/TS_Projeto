using System;
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
            try
            {

                if (textBox.InvokeRequired)
                    textBox.Invoke((MethodInvoker)delegate { textBox.AppendText($"\r\n{msg}"); });
                else
                    textBox.AppendText($"\r\n{msg}");
            }
            catch(Exception ex)
            {
                consoleLog("Unexpected error:\n" + ex.Message);
            }
        }

        //Escreve nova mensagem composta 
        public void newMessage(string owner, string msg)
        {
            string data = $"({owner}): {msg}";
            if (textBox.InvokeRequired)
                textBox.Invoke((MethodInvoker)delegate { textBox.AppendText("\r\n" + data); });
            else
                textBox.AppendText("\r\n" + data);

        }

    }
}

using System;
using System.IO;

// Class LogController para enviar mensagens a consola
class LogController
{
    //Mensagem simple para a consola
    public void consoleLog(string msg)
    {
        // Constroe a mensagem
        msg = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + msg;
        // Guarda a msg no ficheiro
        //this.logFile(msg);
        // Escreve a msg na consola
        Console.WriteLine(msg);
    }

    //Mensagem composta para a consola
    public void consoleLog(string msg, string owner)
    {
        // Constroe a mensagem
        msg = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + "(" + owner + ")" + ": " + msg;
        // Guarda a msg no ficheiro
        //this.logFile(msg);
        // Escreve a msg na consola
        Console.WriteLine(msg);
    }

    //Cria e guarda os logs do servidor
    private void logFile(string msg)
    {
        try
        {
            string path = "log";
            // Constroe o nome do ficheiro
            string file = "chat_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
            // Valida que a diretoria exista
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filepath = path + "\\" + file;
            // Valida se o ficheiro existe
            if (!File.Exists(filepath))
                File.Create(filepath);
            // Guarda a informação no ficheiro
            File.AppendAllText(filepath, "\r\n" + msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

}

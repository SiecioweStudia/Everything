using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Net.Mail;
using System.Net;
namespace SMTP
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] Config = File.ReadAllLines("App.config");
            TcpClient Client = new TcpClient("mkwk018.cba.pl", 21);
            /*
            SmtpClient cl = new SmtpClient(Config[0], Convert.ToInt32(Config[3]));
            MailMessage mes = new MailMessage(Config[1], Config[1], "Mrh", "hurr");
            NetworkCredential cred = new NetworkCredential(Config[1], Config[2]);
            cl.Credentials = cred;
            cl.Send(mes);
*/          byte[] data = new byte[128];
            if (!Client.Connected)
                return;
            NetworkStream Stream = Client.GetStream();
            Stream.ReadTimeout = 60000;
            Stream.Write(data, 0, 128);
            Stream.Read(data, 0, 128);






            System.Console.WriteLine("Refresh");
            if (System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n') != "+OK POP3 ready")
            {
                System.Console.WriteLine("Connection Fail");
                return;
            }


            String USERA = "USER " + Config[1] + "\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(USERA), 0, USERA.Length);
            Stream.Read(data, 0, 128);
            if (System.Text.Encoding.UTF8.GetString(data).Substring(0, 3) != "+OK")
            {
                System.Console.WriteLine("Auth Fail");
                return;
            }
            String PASSA = "PASS " + Config[2] + "\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(PASSA), 0, PASSA.Length);
            Stream.Read(data, 0, 128);
            if (System.Text.Encoding.UTF8.GetString(data).Substring(0, 3) != "+OK")
            {
                System.Console.WriteLine("Auth Fail");
                return;
            }




            String Quit = "QUIT " + "\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(Quit), 0, Quit.Length);
            Stream.Read(data, 0, 128);
            if (System.Text.Encoding.UTF8.GetString(data).Substring(0, 3) != "+OK")
            {
                System.Console.WriteLine("Error while quiting");
                return;
            }
            Client.Close();
        }
    }
}

using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net.Security;
namespace POP3
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            string[] Config =File.ReadAllLines("App.config");


            /*
            Stream.Write(Encoding.ASCII.GetBytes(auth),0,auth.Length);
            Stream.Read(data, 0, 128);
            if (System.Text.Encoding.UTF8.GetString(data).Substring(0, 3) != "+OK")
            {
                System.Console.WriteLine("Auth Fail");
                return;
            }
            */

            String stat= "STAT\r\n";
            int start = -1,end=-1;

            do
            {
                while (!Console.KeyAvailable)
                {
                    TcpClient Client = new TcpClient(Config[0], Convert.ToInt32(Config[3]));
                    NetworkStream Stream = Client.GetStream();
                    byte[] data = new byte[128];
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


                    data = new byte[128];

                    Stream.Write(Encoding.ASCII.GetBytes(stat), 0, stat.Length);

                    Stream.Read(data, 0, 128);

                    string[] statres = System.Text.Encoding.UTF8.GetString(data).Split(' ');
                    //string[] statres = System.Text.Encoding.UTF8.GetString(data).Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    /*

                    int i = 0;
                    for (; i < statres.Length; i++)
                        if (statres[i] == ".") break;
                    if(i==statres.Length)    {
                        System.Console.WriteLine("Too much messaged in box for buffer, consider being a better programmer");
                        return;
                    } 
                    */
                    if (statres[0] == "+OK")
                    {
                        if (start == -1)
                        {
                            start = end = Convert.ToInt32(statres[1]);
                            System.Console.WriteLine("There are {0} messaged in box at start", end);
                        }
                        else if(Convert.ToInt32(statres[1])!=end)
                        {
                            end = Convert.ToInt32(statres[1]);;
                            System.Console.WriteLine("New message (Now: {0})", end);
                        }

                    }
                    else{
                        System.Console.WriteLine("STAT Error");
                        return;
                    }
                    System.Threading.Thread.Sleep(Convert.ToInt32(Config[4])*1000);
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
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);

            System.Console.WriteLine("{0} recived when working", end-start);




            return;               
        }
    }
}

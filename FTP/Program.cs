using System;
using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace FTP
{
    

    class MainClass
    {



        public static byte[] poru=new byte[2];
        public static bool running=true;

        public static void LISTDIR(NetworkStream Stream,string line,int Tree=0){
            
            string purt = "PASV\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(purt), 0, purt.Length);
            byte[] data = new byte[1024];
            Stream.Read(data, 0, 1024);
            if (Tree < 2) System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
            string[] tury = System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n').Split(',');
            poru[1] = Convert.ToByte(tury[4]);
            poru[0] = Convert.ToByte(tury[5].TrimEnd(')', '.'));

            string listfiles = (Tree!=0 ? "LIST"+line.Substring(4) : line ) + "\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(listfiles), 0, listfiles.Length);

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpClient listener = new TcpClient("localhost", BitConverter.ToUInt16(poru, 0));


            byte[] data2 = new byte[1024];
            NetworkStream STR = listener.GetStream();

            data = new byte[1024];
            Stream.Read(data, 0, 1024);
            if (Tree < 2) System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
            if (System.Text.Encoding.UTF8.GetString(data).Substring(0, 3) != "150")
            {
                listener.Close();
                return;
            }
            STR.Read(data2, 0, 1024);
            listener.Close();
            data = new byte[1024];
            Stream.Read(data, 0, 1024);
            if(Tree!=0){
                string[] files= Regex.Split(System.Text.Encoding.UTF8.GetString(data2).TrimEnd('\0', '\r', '\n'), "\r\n|\r|\n");
                foreach(string item in files){
                    if (item == "") continue;
                    if (item[0] == 'd')
                    {
                        Console.WriteLine(new string('\t', Tree - 1) + Regex.Split(item, "(.+)\\s(.+)$")[2]);
                        if(line.Length==4){
                            LISTDIR(Stream, "LIST " + line.Substring(4) + Regex.Split(item, "(.+)\\s(.+)$")[2], Tree + 1);                         
                        }
                        else{
                            LISTDIR(Stream, "LIST" + line.Substring(4) + "/" + Regex.Split(item, "(.+)\\s(.+)$")[2], Tree + 1);  
                        }
                      //  

                    }
                    else Console.WriteLine(new string('\t',Tree-1)+Regex.Split(item,"(.+)\\s(.+)$")[2]);
                }

            }
            else{
                System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data2).TrimEnd('\0', '\r', '\n'));
            }

            if (Tree < 2) System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
        }

        /*
        public static void Fun(){
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpClient listener = new TcpClient("localhost", BitConverter.ToUInt16(poru, 0));

            byte[] data2 = new byte[1024];
            NetworkStream STR = listener.GetStream();
            STR.Read(data2, 0, 1024);
            System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data2).TrimEnd('\0', '\r', '\n'));
            listener.Close();
            return;
        }
        */
        public static void Main(string[] args)
        {



            TcpClient Client = new TcpClient("localhost", 21);

            NetworkStream Stream = Client.GetStream();

            byte[] data = new byte[1024];
            Stream.Read(data, 0, 1024);
            System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
            string user = "USER anonymous\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(user), 0, user.Length);
            data = new byte[1024];
            Stream.Read(data, 0, 1024);
            System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));

            string pass = "PASS anonymous\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(pass), 0, pass.Length);
            data = new byte[1024];
            Stream.Read(data, 0, 1024);
            System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));








            while(running){
                string line = Console.ReadLine();
                if (line.Length == 0)
                {
                    running = false;
                    break;
                }
                if(line.Length < 4){
                    line = line + new string(' ', 4 - line.Length);
                }
                    
                switch(line.ToUpper().Substring(0,4)){
                    case "PWD ":
                        string dir = "PWD\r\n";
                        Stream.Write(Encoding.ASCII.GetBytes(dir), 0, dir.Length);
                        data = new byte[1024];
                        Stream.Read(data, 0, 1024);
                        System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
                        break;
                    case "LIST":
                        LISTDIR(Stream, line);
                        break;
                    case "TREE":
                        LISTDIR(Stream, line, 1);
                        break;
                    case "CWD ":
                        string cwd = line + "\r\n";
                        Stream.Write(Encoding.ASCII.GetBytes(cwd), 0, cwd.Length);
                        data = new byte[1024];
                        Stream.Read(data, 0, 1024);
                        System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
                        break;
                    case "CDUP":
                        string cwup = "CDUP\r\n";
                        Stream.Write(Encoding.ASCII.GetBytes(cwup), 0, cwup.Length);
                        data = new byte[1024];
                        Stream.Read(data, 0, 1024);
                        System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
                        break;
                    default:
                        System.Console.WriteLine("Boing");
                        break;

                }
            }

            string quit = "QUIT\r\n";
            Stream.Write(Encoding.ASCII.GetBytes(quit), 0, quit.Length);
            data = new byte[1024];
            Stream.Read(data, 0, 1024);
            System.Console.WriteLine(System.Text.Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n'));
            Client.Close();    

        }
    }
}

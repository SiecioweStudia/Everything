using System;
using System.Net.Sockets;
using System.IO;

namespace Sieciowe
{
    class MainClass
    {
        static string base64LookUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        static int[] Revbase64LookUp;



        static void Base64Encode(string file){
            byte[] RawData = File.ReadAllBytes(file);
            bool Padded = RawData.Length % 3 == 0 ? false : true;
            char[] Output= new char [Padded ? (RawData.Length / 3 + 1) * 4 : (RawData.Length / 3) * 4];


            int i = 0, j = 0;
            for (; i < RawData.Length-3; j+=4, i+=3){
                int ThreeBytes = (int)RawData[i+2] | ((int)RawData[i + 1] << 8) | ((int)RawData[i] << 16);

                Output[j+3] = base64LookUp[ThreeBytes & 0b00111111];
                Output[j+2] = base64LookUp[(ThreeBytes>>6) & 0b00111111];
                Output[j+1] = base64LookUp[(ThreeBytes >> 12) & 0b00111111];
                Output[j]   = base64LookUp[(ThreeBytes >> 18) & 0b00111111];
            }
            if(Padded){
                if(RawData.Length % 3 == 1){
                    Output[j] = base64LookUp[((int)RawData[i] & 0b11111100)>>2];
                    Output[j + 1] = base64LookUp[((int)RawData[i] & 0b00000011)<<4];
                    Output[j + 2] = '=';
                    Output[j + 3] = '=';
                }
                else{
                    int ThreeBytes = 0 | ((int)RawData[i + 1] << 8) | ((int)RawData[i] << 16);
                    Output[j + 3] = '=';
                    Output[j + 2] = base64LookUp[(ThreeBytes >> 6) & 0b00111111];
                    Output[j + 1] = base64LookUp[(ThreeBytes >> 12) & 0b00111111];
                    Output[j] = base64LookUp[(ThreeBytes >> 18) & 0b00111111];
                }
            }
            FileInfo f = new FileInfo(file);
            string header;
            if (f.Extension == ".zip")
            {
                header = "bin/zip;base64,";
            }
            else if (f.Extension == ".txt")
            {
                header = "text/plain;base64,";
            }
            else{
                header = "image/" + f.Extension.Substring(1) + ";base64,";
            }
            File.WriteAllText(f.DirectoryName + "/" + Path.GetFileNameWithoutExtension(f.Name) + ".b64", header + new string(Output, 0, Output.Length));
            
        }

        static void Base64Decode(string file)
        {
            if(Revbase64LookUp==null){
                Revbase64LookUp = new int[256];
                for (int i = 0; i < 64;i++){
                    Revbase64LookUp[base64LookUp[i]] = i; 
                }
            }

            string b64 = File.ReadAllText(file);
            string[] SplitFile= b64.Split(',');
            if (SplitFile.Length > 2)
                return;
            string Header = SplitFile[0];
            string Data = SplitFile[1];
            int OutputLength = (Data.Length / 4) * 3;

            byte[] RawOutput= new byte [OutputLength];

            for (int i=0,j=0;i<Data.Length ;i+=4,j+=3)
            {
                int ThreeBytes = (Revbase64LookUp[Data[i]] << 18) | (Revbase64LookUp[Data[i + 1]] << 12) | (Revbase64LookUp[Data[i + 2]] << 6) | (Revbase64LookUp[Data[i + 3]]);

                RawOutput[j + 2] = (byte)(ThreeBytes & 255);
                RawOutput[j+1] = (byte)((ThreeBytes & 65280 )>>8);
                RawOutput[j] =(byte)(( ThreeBytes & 16711680) >>16);
            }

            FileInfo f = new FileInfo(file);
            if (Data[Data.Length - 1] == '=') OutputLength--;
            if (Data[Data.Length - 2] == '=') OutputLength--;
            if(Header.Substring(0,10)=="text/plain"){
                File.WriteAllText(f.DirectoryName + "/" + Path.GetFileNameWithoutExtension(f.Name) + ".txt",System.Text.Encoding.UTF8.GetString(RawOutput, 0, OutputLength));
            }
            else{
                Array.Resize(ref RawOutput, OutputLength);
                File.WriteAllBytes(f.DirectoryName + "/" + Path.GetFileNameWithoutExtension(f.Name) + (Header.Split('/',';')[1]), RawOutput );
            }

        }

        public static void Main(string[] args)
        {
            Base64Decode("Small.b64");
            Console.WriteLine("Hello World!");
        }
    }
}

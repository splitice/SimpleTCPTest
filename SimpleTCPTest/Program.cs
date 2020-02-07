using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTCPTest
{
    class Program
    {
        private static String mode = "count";
        private static int i = 0;

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (args[0] == "long")
                {
                    mode = args[0];
                }
            }

            TcpListener listener = new TcpListener(8888);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Accepting new TCP connection");

                Thread thread = new Thread(ClientMain);
                thread.Start(client);
            } 
        }

        private static void ClientMain(object obj)
        {
            try
            {
                TcpClient client = obj as TcpClient;
                var stream = client.GetStream();
                StreamReader sr = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                DateTime last = DateTime.Now;

                while (client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        Console.WriteLine("Reading");
                        var line = sr.ReadLine();
                        sw.WriteLine("Ack: \"{0}\"", line);
                    }

                    Thread.Sleep(100);
                    if (DateTime.Now - last > TimeSpan.FromSeconds(1))
                    {
                        sw.WriteLine(GetTransmission());
                        last = DateTime.Now;
                    }

                    sw.Flush();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetTransmission()
        {
            if (mode == "count")
            {
                return (++i).ToString();
            }

            if (mode == "long")
            {
                return "".PadLeft(i++, 'X');
            }

            return "unknown";
        }
    }
}

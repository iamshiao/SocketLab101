using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SyncSocket;

namespace SyncSocketSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(3000);
            int errorCount = 0;
            for (int i = 0; i < 100; i++) {
                try {
                    SenderSite ss = new SenderSite();
                    Socket sender = ss.Connect("127.0.0.1", 11000);
                    //ss.SendStr(sender, "Test");
                    //ss.SendFile(sender, @"C:\Users\Administrator\Pictures\3116.jpg");
                    List<string> paths = new List<string> {
                        @"C:\Users\Administrator\Pictures\3116.jpg",
                        @"C:\Users\Administrator\Pictures\pika.jpg"
                    };
                    ss.SendFiles(sender, paths);
                    SocketPack pack = ss.Recv(sender);
                    if (pack.DataType == "String") {
                        string msg = Encoding.UTF8.GetString(pack.DataBody);
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss") } - {msg}");
                    }
                }
                catch (Exception) {
                    Console.WriteLine($"Error count {++errorCount}.");
                }

                Thread.Sleep(new Random(DateTime.Now.Second).Next(1, 7) * 100);
            }

            Console.ReadLine();
        }
    }
}

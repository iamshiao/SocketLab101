using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SyncSocket;

namespace SyncSocketListener
{
    class Program
    {
        static void Main(string[] args)
        {
            ListenerSite ls = new ListenerSite();
            ls.InjectedBehave = (handler) => {
                SocketPack pack = ls.Recv(handler);
                if (pack.DataType == "String") {
                    string msg = Encoding.UTF8.GetString(pack.DataBody);
                    Console.WriteLine(msg);
                    ls.SendStr(handler, "OK");
                }
                else if (pack.DataType == "File") {
                    File.WriteAllBytes($@"C:\(Socket){pack.FileName}", pack.DataBody);
                    ls.SendStr(handler, "File OK");
                }
                else if (pack.DataType == "Files") {
                    foreach (SocketPack subPack in pack.SubPacks) {
                        File.WriteAllBytes($@"C:\(Socket){subPack.FileName}", subPack.DataBody);
                    }
                    ls.SendStr(handler, "Files OK");
                }
            };

            Thread lt = new Thread(new ThreadStart(() => {
                ls.Activate("127.0.0.1", 11000);
            }));
            lt.Start();

            for (int i = 0; i < 100; i++) {
                Console.WriteLine($"Main Thread counting - {i}");
                Thread.Sleep(500);
            }
        }
    }
}

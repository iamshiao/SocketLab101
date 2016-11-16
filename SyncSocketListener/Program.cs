using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SyncSocket;

namespace SyncSocketListener
{
    class Program
    {
        static Thread _lt;

        static void Main(string[] args)
        {
            NetworkChange.NetworkAvailabilityChanged += (sender, e) => {
                if (e.IsAvailable && _lt == null) {
                    SetListener();
                }
            };

            if (NetworkInterface.GetIsNetworkAvailable()) {
                SetListener();
            }

            while (true) {
                Console.WriteLine($"Simulate that 【Main Thread】 is doing something else.");
                Thread.Sleep(5000);
            }
        }

        private static void SetListener()
        {
            ListenerSite ls = new ListenerSite();
            ls.InjectedBehave = (handler) => {
                SocketPack pack = ls.Recv(handler);
                if (pack.DataType == "String") {
                    string msg = Encoding.UTF8.GetString(pack.DataBody);
                    Console.WriteLine($"Msg from client: {msg}");
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

            _lt = new Thread(new ThreadStart(() => {
                ls.Activate("127.0.0.1", 11000);
            }));
            _lt.IsBackground = true;
            _lt.Start();
        }
    }
}

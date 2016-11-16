using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;

namespace SyncSocket
{
    public class ListenerSite : GeneralOperation
    { 
        public Action<Socket> InjectedBehave { get; set; }

        public void Activate(string ip, int port)
        {
            try {
                // Establish the local endpoint for the socket.
                // Dns.GetHostName returns the name of the 
                // host running the application.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                //IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(
                //            info => info.AddressFamily == AddressFamily.InterNetwork);
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and 
                // listen for incoming connections.

                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true) {
                    Console.WriteLine("Waiting...");
                    // Program is suspended(blocked) while waiting for an incoming connection.
                    // ★ handler is an object help you communicate with caller; It is neither caller nor listener.
                    Socket handler = listener.Accept();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - New Connected.");

                    try {
                        InjectedBehave(handler);
                    }
                    catch (Exception) {
                    }

                    ShutDownAndClose(handler);
                }

            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

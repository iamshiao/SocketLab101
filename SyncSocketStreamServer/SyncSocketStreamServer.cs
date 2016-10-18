using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketLab101
{
    public class SyncSocketStreamServer
    {
        // Incoming data from the client.
        public static string data = null;

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(
                        info => info.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true) {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended(blocked) while waiting for an incoming connection.
                    // ★ handler is an object help you communicate with caller; It is neither caller nor listener.
                    Socket handler = listener.Accept();

                    using (NetworkStream stream = new NetworkStream(handler)) {
                        using (StreamWriter sw = new StreamWriter(stream)) {
                            sw.WriteLine("This is a Call");
                            sw.Flush();
                        }
                    }
                    using (NetworkStream stream = new NetworkStream(handler)) {
                        using (StreamReader sr = new StreamReader(stream)) {
                            Console.WriteLine("Recv: " + sr.ReadLine());
                        }
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}

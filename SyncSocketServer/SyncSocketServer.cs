using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketLab101
{
    public class SyncSocketServer
    {
        // Incoming data from the client.
        public static string data = null;

        public static void StartListening()
        {
            // Data buffer for incoming data. 1kb per Recv
            byte[] bytes = new Byte[1024];

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
                    data = null;

                    int bytesRec = 0;
                    // An incoming connection needs to be processed.
                    while (true) {
                        bytes = new byte[1024];
                        bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec); // 1k - bytesRec = wasted bytes
                        if (data.IndexOf("<EOF>") > -1) {
                            break;
                        }
                    }

                    // Show the data on the console.
                    Console.WriteLine($"Recev: {data}");

                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes("This is a Reply");
                    handler.Send(msg);

                    // Continuously Send will automatically combine to one which include both data bytes
                    //byte[] msg2 = Encoding.ASCII.GetBytes("This is a Reply2");
                    //handler.Send(msg2);

                    // Times of Send & Recv per connection is unlimited, 
                    // and both side could be last Sender or last Receiver
                    bytesRec = handler.Receive(bytes);
                    Console.WriteLine($"Recv: {Encoding.ASCII.GetString(bytes, 0, bytesRec)}");

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

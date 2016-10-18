using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SocketLab101
{
    internal class StateObject
    {
        /// <summary>Handle for communicate to client</summary>
        public Socket Handler = null;

        /// <summary>Buffer size</summary>
        public const int BufferSize = 1024;

        /// <summary>Buffer of each send</summary>
        public byte[] BufferPerSend = new byte[BufferSize];

        /// <summary>Stream for send</summary>
        public NetworkStream SendStream { get; set; }

        /// <summary>Stream for recv</summary>
        public NetworkStream RecvStream { get; set; }
    }

    public class AsyncSocketStreamServer
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsyncSocketStreamServer()
        {
        }

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(
                        info => info.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true) {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.Handler = handler;
            state.RecvStream = new NetworkStream(state.Handler);
            if (state.RecvStream.CanRead) {
                state.RecvStream.BeginRead(state.BufferPerSend, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
            }
            else {
                Console.WriteLine("Can't Recv from NetworkStream");
            }
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.Handler;

            // Read data from the client socket. 
            int bytesRead = state.RecvStream.EndRead(ar);

            while (state.RecvStream.DataAvailable) {
                state.RecvStream.BeginRead(state.BufferPerSend, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
                data += Encoding.ASCII.GetString(state.BufferPerSend, 0, bytesRead);
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            StateObject state = new StateObject();
            state.SendStream = new NetworkStream(handler);

            state.SendStream.BeginWrite(byteData, 0, byteData.Length, new AsyncCallback(SendCallback), state);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try {
                // Retrieve the socket from the state object.
                StateObject state = (StateObject)ar.AsyncState;

                // Complete sending the data to the remote device.
                using (state.SendStream) {
                    state.SendStream.EndWrite(ar);
                }

                state.Handler.Shutdown(SocketShutdown.Both);
                state.Handler.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}

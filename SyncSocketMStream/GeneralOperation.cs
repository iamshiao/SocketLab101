using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace SyncSocket
{
    public class GeneralOperation
    {
        /// <summary>Send string</summary>
        /// <param name="handler">handler of sender or sender itself</param>
        /// <param name="msg">string message</param>
        public void SendStr(Socket handler, string msg)
        {
            try {
                byte[] data;
                using (MemoryStream stream = new MemoryStream()) {
                    byte[] body = Encoding.UTF8.GetBytes(msg);
                    SocketPack pack = new SocketPack
                    {
                        DataType = "String",
                        FileName = "",
                        DataBody = body
                    };
                    var formater = new DataContractSerializer(typeof(SocketPack));
                    formater.WriteObject(stream, pack);
                    data = stream.ToArray();
                }
                Thread.Sleep(100);
                handler.Send(data);
            }
            catch (Exception ex) {
                ShutDownAndClose(handler);
            }
        }

        /// <summary>Send file</summary>
        /// <param name="handler">handler of sender or sender itself</param>
        /// <param name="fullPath">file full path</param>
        public void SendFile(Socket handler, string fullPath)
        {
            try {
                if (File.Exists(fullPath)) {
                    byte[] data;
                    using (MemoryStream stream = new MemoryStream()) {

                        string fileName = Path.GetFileName(fullPath);
                        byte[] body = File.ReadAllBytes(fullPath);
                        SocketPack pack = new SocketPack
                        {
                            DataType = "File",
                            FileName = fileName,
                            DataBody = body
                        };
                        var formater = new DataContractSerializer(typeof(SocketPack));
                        formater.WriteObject(stream, pack);
                        data = stream.ToArray();
                    }
                    Thread.Sleep(100);
                    handler.Send(data);
                }
            }
            catch (Exception ex) {
                ShutDownAndClose(handler);
            }
        }

        /// <summary>Send files</summary>
        /// <param name="handler">handler of sender or sender itself</param>
        /// <param name="fullPaths">files full paths</param>
        public void SendFiles(Socket handler, List<string> fullPaths)
        {
            try {
                List<SocketPack> subPacks = new List<SocketPack>();
                foreach (string path in fullPaths) {
                    if (File.Exists(path)) {
                        string fileName = Path.GetFileName(path);
                        byte[] body = File.ReadAllBytes(path);
                        SocketPack pack = new SocketPack
                        {
                            DataType = "File",
                            FileName = fileName,
                            DataBody = body
                        };
                        subPacks.Add(pack);
                    }
                }

                byte[] data;
                using (MemoryStream stream = new MemoryStream()) {
                    SocketPack pack = new SocketPack
                    {
                        DataType = "Files",
                        SubPacks = subPacks
                    };
                    var formater = new DataContractSerializer(typeof(SocketPack));
                    formater.WriteObject(stream, pack);
                    data = stream.ToArray();
                }
                Thread.Sleep(100);
                handler.Send(data);
            }
            catch (Exception ex) {
                ShutDownAndClose(handler);
            }
        }

        /// <summary>Recv</summary>
        /// <param name="handler">handler of sender or sender itself</param>
        public SocketPack Recv(Socket handler)
        {
            SocketPack recvPack = null;
            try {
                byte[] bytes = new byte[30 * 1024 * 1024];
                int certainSize = handler.Receive(bytes);
                Array.Resize(ref bytes, certainSize);

                using (MemoryStream stream = new MemoryStream(bytes)) {
                    var serializer = new DataContractSerializer(typeof(SocketPack));
                    recvPack = (SocketPack)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex) {
                ShutDownAndClose(handler);
            }

            return recvPack;
        }

        /// <summary>Shut down and close socket</summary>
        public void ShutDownAndClose(Socket handler)
        {
            try {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (ObjectDisposedException) {
            }
        }
    }
}

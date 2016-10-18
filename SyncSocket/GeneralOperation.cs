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
                using (NetworkStream stream = new NetworkStream(handler)) {
                    byte[] body = Encoding.UTF8.GetBytes(msg);
                    SocketPack pack = new SocketPack
                    {
                        DataType = "String",
                        FileName = "",
                        DataBody = body
                    };
                    var formater = new BinaryFormatter();
                    formater.Serialize(stream, pack);

                    stream.Flush();
                }
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
                using (NetworkStream stream = new NetworkStream(handler)) {
                    if (File.Exists(fullPath)) {
                        string fileName = Path.GetFileName(fullPath);
                        byte[] body = File.ReadAllBytes(fullPath);
                        SocketPack pack = new SocketPack
                        {
                            DataType = "File",
                            FileName = fileName,
                            DataBody = body
                        };
                        var formater = new BinaryFormatter();
                        formater.Serialize(stream, pack);
                        stream.Flush();
                    }
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

                using (NetworkStream stream = new NetworkStream(handler)) {
                    SocketPack pack = new SocketPack
                    {
                        DataType = "Files",
                        SubPacks = subPacks
                    };
                    var formater = new BinaryFormatter();
                    formater.Serialize(stream, pack);
                    stream.Flush();
                }
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
                using (NetworkStream stream = new NetworkStream(handler)) {
                    var formatter = new BinaryFormatter();
                    recvPack = (SocketPack)formatter.Deserialize(stream);
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

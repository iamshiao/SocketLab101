using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyncSocket
{
    public class SenderSite : GeneralOperation
    {
        public Socket Connect(string ip, int port)
        {
            Socket sender = null;
            try {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                //IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(
                //    info => info.AddressFamily == AddressFamily.InterNetwork);
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(remoteEP);
            }
            catch (Exception ex) {
                ShutDownAndClose(sender);
            }

            return sender;
        }
    }
}

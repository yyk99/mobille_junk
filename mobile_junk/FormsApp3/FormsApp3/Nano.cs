using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using System.Threading.Tasks;

namespace FormsApp3
{
    struct Record
    {
        public UInt32 stamp;
        public Int32 x, y, z;
        public UInt32 h, c, f;
    };
    class Nano : IDisposable
    {
        UdpClient client;
        IPEndPoint endPoint;

        public Nano(string host = "192.168.1.136", int port = 2390)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            // Create a UDP/IP socket.  
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            endPoint = new IPEndPoint(ipAddress, port);
            // client.Connect(endPoint);
        }

        public async Task Ping()
        {
            await Task.Run(() =>
            {
                byte[] msgOpen = Encoding.ASCII.GetBytes("CONNECT\r\n");
                int ok = client.Send(msgOpen, msgOpen.Length, endPoint);

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = client.Receive(ref remote);

                Console.WriteLine("Received: {0}", Encoding.ASCII.GetString(receiveBytes));

                byte[] msgClose = Encoding.ASCII.GetBytes("CLOSE\r\n");
                ok = client.Send(msgClose, msgClose.Length, endPoint);
            });
        }

        public async Task<Record> GetRecord()
        {
            return await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine($"GetRecord: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                byte[] msgOpen = Encoding.ASCII.GetBytes("CONNECT\r\n");
                int ok = client.Send(msgOpen, msgOpen.Length, endPoint);

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            again:
                byte[] receiveBytes = client.Receive(ref remote);

                System.Diagnostics.Debug.WriteLine($"Received: {receiveBytes.Length} bytes");
                System.Diagnostics.Debug.WriteLine($"Received: {Encoding.ASCII.GetString(receiveBytes)}");

                if (Encoding.ASCII.GetString(receiveBytes) != "OK Connected\r\n")
                    goto again;

                receiveBytes = client.Receive(ref remote);
                System.Diagnostics.Debug.WriteLine($"Received: {receiveBytes.Length} bytes");

                // unpack the data
                // BitConverter.ToInt32

                Record res = new Record {
                    stamp = BitConverter.ToUInt32(receiveBytes, 0),
                    x = BitConverter.ToInt32(receiveBytes, 4),
                    y = BitConverter.ToInt32(receiveBytes, 8),
                    z = BitConverter.ToInt32(receiveBytes, 12),
                    h = BitConverter.ToUInt32(receiveBytes, 16),
                    c = BitConverter.ToUInt32(receiveBytes, 20),
                    f = BitConverter.ToUInt32(receiveBytes, 24)
                };

                //byte[] msgClose = Encoding.ASCII.GetBytes("CLOSE\r\n");
                //ok = client.Send(msgClose, msgClose.Length, endPoint);

                return res;
            });
        }

        public void Dispose()
        {
            ((IDisposable)client).Dispose();
        }
    }
}

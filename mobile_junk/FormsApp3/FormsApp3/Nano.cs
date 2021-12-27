using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using System.Threading.Tasks;

namespace Embedded
{
    struct Record
    {
        public System.UInt32 stamp;
        public System.Int32 x, y, z;
        public System.UInt32 h, c, f;
    };
    class Nano
    {
        IPEndPoint endPoint;
        const int TIMEOUT = 10 * 1000;

        public Nano(string host = "192.168.1.136", int port = 2390)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            endPoint = new IPEndPoint(ipAddress, port);
            // client.Connect(endPoint);
        }

        public async Task<string> Ping()
        {

            using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                byte[] msgOpen = Encoding.ASCII.GetBytes("CONNECT\r\n");
                int ok = client.Send(msgOpen, msgOpen.Length, endPoint);

                var rcv_task = client.ReceiveAsync();
                if (await Task.WhenAny(rcv_task, Task.Delay(TIMEOUT)) != rcv_task)
                {
                    // timeout!
                    client.Close();
                    return "BAD: Timeout";
                }
                byte[] receiveBytes = rcv_task.Result.Buffer;

                byte[] msgClose = Encoding.ASCII.GetBytes("CLOSE\r\n");
                client.Send(msgClose, msgClose.Length, endPoint);

                return Encoding.ASCII.GetString(receiveBytes);
            }
        }

        public async Task<Record> GetRecord()
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                System.Diagnostics.Debug.WriteLine($"GetRecord: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                byte[] msgOpen = Encoding.ASCII.GetBytes("CONNECT\r\n");
                int ok = client.Send(msgOpen, msgOpen.Length, endPoint);

                var rcv_task = client.ReceiveAsync();
                if (await Task.WhenAny(rcv_task, Task.Delay(TIMEOUT)) != rcv_task)
                {
                    // timeout!
                    client.Close();
                    return new Record { stamp = 0x0 };
                }
                UdpReceiveResult received = rcv_task.Result;
                var receiveBytes = received.Buffer;

                System.Diagnostics.Debug.WriteLine($"Received: {receiveBytes.Length} bytes");
                System.Diagnostics.Debug.WriteLine($"Received: {Encoding.ASCII.GetString(receiveBytes)}");

                rcv_task = client.ReceiveAsync();
                if (await Task.WhenAny(rcv_task, Task.Delay(TIMEOUT)) != rcv_task)
                {
                    // timeout!
                    client.Close();
                    return new Record { stamp = 0x0 };
                }

                received = rcv_task.Result;
                receiveBytes = received.Buffer;
                System.Diagnostics.Debug.WriteLine($"Received: {receiveBytes.Length} bytes");

                // unpack the data
                Record res = new Record
                {
                    stamp = BitConverter.ToUInt32(receiveBytes, 0),
                    x = BitConverter.ToInt32(receiveBytes, 4),
                    y = BitConverter.ToInt32(receiveBytes, 8),
                    z = BitConverter.ToInt32(receiveBytes, 12),
                    h = BitConverter.ToUInt32(receiveBytes, 16),
                    c = BitConverter.ToUInt32(receiveBytes, 20),
                    f = BitConverter.ToUInt32(receiveBytes, 24)
                };

                byte[] msgClose = Encoding.ASCII.GetBytes("CLOSE\r\n");
                client.Send(msgClose, msgClose.Length, endPoint);

                client.Close();
                return res;
            }
        }

        public IPEndPoint RemoteEndpoint
        {
            get { return endPoint; }
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using System.Threading.Tasks;

namespace get_nano_udp
{
    struct Record
    {
        public UInt32 stamp;
        public Int32 x, y, z;
        public UInt32 h, c, f;
    };
    class Nano
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
                byte[] msgOpen = Encoding.ASCII.GetBytes("CONNECT\r\n");
                int ok = client.Send(msgOpen, msgOpen.Length, endPoint);

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            again:
                Byte[] receiveBytes = client.Receive(ref remote);
                
                Console.WriteLine("Received: {0} bytes", receiveBytes.Length);
                Console.WriteLine("Received: {0}", Encoding.ASCII.GetString(receiveBytes));

                if (Encoding.ASCII.GetString(receiveBytes) != "OK Connected\r\n")
                    goto again;

                receiveBytes = client.Receive(ref remote);
                Console.WriteLine("Received: {0} bytes", receiveBytes.Length);

                // unpack the data
                // BitConverter.ToInt32

                var res = new Record();
                res.stamp = BitConverter.ToUInt32(receiveBytes, 0);
                res.x = BitConverter.ToInt32(receiveBytes, 4);
                res.y = BitConverter.ToInt32(receiveBytes, 8);
                res.z = BitConverter.ToInt32(receiveBytes, 12);
                res.h = BitConverter.ToUInt32(receiveBytes, 16);
                res.c = BitConverter.ToUInt32(receiveBytes, 20);
                res.f = BitConverter.ToUInt32(receiveBytes, 24);

                byte[] msgClose = Encoding.ASCII.GetBytes("CLOSE\r\n");
                ok = client.Send(msgClose, msgClose.Length, endPoint);


                return res;
            });
        }
    }
    class Nano2
    {
        public Nano2(string host = "192.168.1.136", int port = 2390)
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Nano nano = new Nano();
#if false
            var t = nano.Ping();

            if (t.Wait(10 * 1000))
                Console.WriteLine("Nano is alive!");
            else
                Console.WriteLine("Ping failed (timeout)");
#endif

            var t2 = nano.GetRecord();
            if (t2.Wait(10 * 1000))
            {
                var r = t2.Result;
                Console.WriteLine("{0}% {1}C {2}F", r.h / 100.00, r.c / 100.00, r.f / 100.00);
            }
            else
            {
                Console.WriteLine("GetRecord failed (timeout)");
            }
        }
    }
}

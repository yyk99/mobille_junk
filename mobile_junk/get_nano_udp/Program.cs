using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace get_nano_udp
{
    class Nano
    {
        UdpClient client;

        public Nano(string host = "pino.home", int port = 2390)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("pino.home");
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            // Create a UDP/IP socket.  
            client = new UdpClient();
            client.Connect(ipAddress, port);
        }

        public void ping()
        {
            byte[] byteData = Encoding.ASCII.GetBytes("CONNECT\r\n");
            int ok = client.Send(byteData, byteData.Length);
            // assert ok == length(bytedata)

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = client.Receive(ref remote);

            Console.WriteLine("Received: {0}", ok);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Nano pino = new Nano();

            pino.ping();

            Console.WriteLine("Hello from Pino!");
        }
    }
}

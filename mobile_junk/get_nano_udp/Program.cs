using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using System.Threading.Tasks;

using Embedded;

namespace get_nano_udp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Nano nano = new Nano();

            {
                var t = nano.Ping();
                Console.WriteLine("PING: {0}", t.Result.TrimEnd());
            }
            {
                var t2 = nano.GetRecord();
                var r = t2.Result;
                if (r.stamp != 0)
                    Console.WriteLine("{0}% {1}C {2}F", r.h / 100.00, r.c / 100.00, r.f / 100.00);
                else
                    Console.WriteLine("GetRecord failed (timeout)");
            }
        }
    }
}

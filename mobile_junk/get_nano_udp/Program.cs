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
#if false
            var t = nano.Ping();

            if (t.Wait(10 * 1000))
                Console.WriteLine("Nano is alive!");
            else
                Console.WriteLine("Ping failed (timeout)");
#endif

            var t2 = nano.GetRecord();
            int tmo = 10 * 1000; // ten seconds
            if (t2.Wait(tmo))
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

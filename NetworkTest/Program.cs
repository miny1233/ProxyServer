using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NetworkTest
{
    class Program
    {
        static Socket[] tcpc,udpc;
        static Thread t, u;
        static object tcp, udp;
        static void Main(string[] args)
        {
            tcpc = new Socket[3];
            udpc = new Socket[3];
            tcpc[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpc[1] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            udpc[0] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpc[1] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            tcp = tcpc;
            udp = udpc;
            t = new Thread(Proxy.TcpProxy);
            u = new Thread(Proxy.UdpProxy);
            t.Start(tcp);
            u.Start(udp);
        }
        
    }
    class Proxy
    {
       public static void UdpProxy(object u)
        {
            Console.WriteLine("Udp Start");
            EndPoint end = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 25565);
            byte[] Pack = new byte[1024];
            Socket[] socketss = (Socket[])u;
            socketss[0].Bind(new IPEndPoint(IPAddress.Parse("192.168.0.100"), 855));
            socketss[1].Bind(new IPEndPoint(IPAddress.Parse("192.168.0.100"), 856));
            while (true)
            {
                socketss[0].Receive(Pack);
                socketss[1].SendTo(Pack, end);
            }
        }
        static Thread th;
        static Socket[] sockets;
        public static void TcpProxy(object t)
        {
            Console.WriteLine("TCP Start");
            EndPoint end = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 25565);
            byte[] Pack = new byte[1024];
            sockets = (Socket[])t;
            sockets[0].Bind(new IPEndPoint(IPAddress.Parse("192.168.0.100"), 855));
            sockets[1].Bind(new IPEndPoint(IPAddress.Parse("192.168.0.100"), 856));
            sockets[0].Listen(1);
            sockets[2] = sockets[0].Accept();
            sockets[1].Connect(end);
            th = new Thread(TBP);
            object aa = sockets;
            th.Start();
                while (true)
                {
                    sockets[2].Receive(Pack);
                    sockets[1].Send(Pack);
                }
            }
        public static void TBP()
        {           
            byte[] Pack = new byte[1024];
            while (true)
            { 
                sockets[1].Receive(Pack);
                sockets[2].Send(Pack);
            }
        }
        }
    }

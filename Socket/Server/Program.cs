using System;
using System.Collections.Generic;
using System.Diagnostics;

//using Internal;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            JsonManager json = new JsonManager();
            json.LoadJson();
            List<string> IpList = json.GetIP();

            UDPServer UDPserver;
            TCPServer TCPserver;

            Stopwatch stopwatch = new Stopwatch();
            byte status;

            int seconds = 0;

            while (true)
            {
                Console.WriteLine("UDP SERVER OPEN");
                UDPserver = new UDPServer();
                UDPserver.Bind();
                try
                {
                    while (true)
                    {
                        status = UDPserver.ReceiveFrom();
                        UDPserver.Write();
                        if (status == 0x01) break;
                        UDPserver.SendTo();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }

                Console.WriteLine("UDP SERVER CLOSE");
                UDPserver.ServerClose();

                Console.WriteLine("TCP SERVER OPEN");
                TCPserver = new TCPServer();
              
                TCPserver.Bind();
                TCPserver.Listen();

                TCPserver.Accept(IpList);
                stopwatch.Start();
                while (true)
                {
                    seconds = (int)(stopwatch.ElapsedMilliseconds/1000);
                    if (seconds > 3)
                    {
                        Console.WriteLine("3초 지남");
                        break;
                    }
                    TCPserver.Read();
                    //아무것도 오지 않으면 => 연결이 끊키면
                    if (TCPserver.retval == 0) break;
                    TCPserver.Write();
                    TCPserver.Send();
                }
                Console.WriteLine("TCP SERVER CLOSE");
                TCPserver.ClientSockClose();
                TCPserver.ServerClose();
                stopwatch.Stop();
                stopwatch.Reset();
                seconds = 0;
                Console.WriteLine("==========================================================================");
            }
        }
    }
}
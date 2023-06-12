using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPClient UDPclient;
            TCPClient TCPclient;
            CLIUI cli = new CLIUI();
            string IP = null;
            string MSG = null;
            int PORT = 0;

            bool connectStatus = false;

            int inputNum = 0;

            bool isClose;

            while (true)
            {
                UDPclient = new UDPClient();

                //UDP를 통한 TCP 접속시도
                while (true)
                {
                    IP = cli.QIP();
                    PORT = cli.QPORT();

                    UDPclient.trySend(IP, PORT);
                    int chek = UDPclient.Read();
                    UDPclient.SendOK(1);
                    if (chek == 1)
                    {
                        break;
                    }
                }

                UDPclient.Close();

                TCPclient = new TCPClient();
                connectStatus = TCPclient.Connect(IP);

                Console.WriteLine("Enter 입력시 접속 종료");
                if (connectStatus)
                {
                    while (true)
                    {
                        isClose = TCPclient.Write();
                        isClose = TCPclient.Read();
                        //Enter시 접속 종료
                        if (!isClose)
                        {
                            TCPclient.Read();
                            TCPclient.Close();
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("접속 거부");
                }
            }
        }
    }
}
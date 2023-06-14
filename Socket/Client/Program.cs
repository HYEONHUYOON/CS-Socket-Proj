using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

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

            int tcpPORT;

            while (true)
            {
                UDPclient = new UDPClient();

                //UDP를 통한 TCP 접속시도
                while (true)
                {
                    while (true) {
                        IP = cli.QIP();
                        PORT = cli.QPORT();

                        bool connect = UDPclient.trySend(IP, PORT);
                        if(connect)break;
                    }
                    bool check = UDPclient.Read();
                    if(check) { 
                    UDPclient.SendOK(1);
                        }
                    if (check == true)break;
                }

                tcpPORT = UDPclient.tcpPort;
                UDPclient.Close();

                TCPclient = new TCPClient();
                connectStatus = TCPclient.Connect(IP, tcpPORT);

                if (connectStatus)
                {
                    while (true)
                    {
                        //isClose = TCPclient.Write();
                        if (TCPclient.Read())
                        {
                            break;
                        }
                    }
                    TCPclient.Close();

                    Console.WriteLine("서버와 통신이 종료");
                }
                else
                {
                    Console.WriteLine("접속 거부");
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    class UDPServer
    {
        const int BUFSIZE = 512;
        const int SERVERPORT = 3;

        public int retval;
        Socket listen_sock = null;

        // 데이터 통신에 사용할 변수
        IPEndPoint anyaddr = null;
        EndPoint peeraddr = null;
        byte[] buf = new byte[BUFSIZE];

        //로그 파일
        StreamWriter sw;

        bool isWhiteList = false;

        string token1;
        string token2;

        JObject jsonData;

        public UDPServer()
        {
            try
            {
                sw = File.AppendText(@"C:\Users\윤현후\source\repos\Socket\Server\ServerLog.txt");
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => " + $"{"SERVER OPEN",-15}");
                sw.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Environment.Exit(1);
            }
            try
            {
                // UDP 소켓 생성
                listen_sock = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Environment.Exit(1);
            }
        }

        public void Bind()
        {
            try
            {
                // Bind()
                listen_sock.Bind(new IPEndPoint(IPAddress.Any, SERVERPORT));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public byte ReceiveFrom()
        {
            Array.Clear(buf);
            retval = 0;
            try
            {
                anyaddr = new IPEndPoint(IPAddress.Any, 3);
                peeraddr = (EndPoint)anyaddr;

                // 데이터 받기
                retval = listen_sock.ReceiveFrom(buf, 55,
                       SocketFlags.None, ref peeraddr);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => "
                   + $"{"RECEIVE",-15} IP : {anyaddr.Address,-15} PORT : {anyaddr.Port,-10} {Encoding.Default.GetString(buf)}");
                sw.Flush();

                string receiveToken = Encoding.Default.GetString(buf);
                Console.WriteLine(receiveToken);

                if(string.Compare(receiveToken,"ACK OK",true) == 0)
                {
                    Console.WriteLine("통과");
                    return 0x01;
                }
                else
                {
                    //JSON 파싱
                    jsonData = JObject.Parse(receiveToken);
                    string jtoken = jsonData["요청 TOKEN"].ToString();    
                    string[] tokens = jtoken.Split('-');

                    token1 = tokens[1];
                    token2 = tokens[3];

                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => "
                   + $"{"TOKEN",-15} IP : {anyaddr.Address,-15} PORT : {anyaddr.Port,-10} {token1+token2}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ReceiveFrom");
                Console.WriteLine(e.Message);
            }
            return 0x00;
        }
        public void Write()
        {
            try
            {
                // 받은 데이터 출력
                Console.WriteLine("[UDP/{0}:{1}] {2}",
                    anyaddr.Address, anyaddr.Port,
                    Encoding.Default.GetString(buf, 0, retval));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SendTo()
        {
            //token  생성
            Random random = new Random();
            string[] randomNumber = new string[2];
            for (int i = 0; i < 2; i++)
            {
                randomNumber[i] = random.Next(1000, 10000).ToString();
            }
            string TokenPort = SERVERPORT.ToString("D4");    
            string token = token2 + '-' + randomNumber[0] + '-' + TokenPort + '-' + randomNumber[1] + '-' + token1;
            string tokenjson = "{\r\n    \"TYPE\" : \"TCP ,  응답 TOKEN\",\r\n    \"응답 TOKEN\" : \"" + token + "\"\r\n}";

            try
            {
                buf = Encoding.Default.GetBytes(tokenjson);
                retval = buf.Length;
                // 데이터 보내기
                listen_sock.SendTo(buf, 0, retval, SocketFlags.None, peeraddr);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => "
                 + $"{"SEND",-15} IP : {anyaddr.Address,-15} PORT : {anyaddr.Port,-10} {Encoding.Default.GetString(buf)}");
                sw.Flush();

                Console.WriteLine("[SEND] : {0}", Encoding.Default.GetString(buf, 0, retval));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("SendTo");
            }
        }

        public void ServerClose()
        {
            try
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => " + $"{"SERVER CLOSE",-15}");
                sw.Close();
                // 소켓 닫기
                listen_sock.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TokenParse()
        {

        }
    }
}

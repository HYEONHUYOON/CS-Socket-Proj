using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Client;


namespace Client
{
    public class token
    {
        public string 요청토큰 { get; set; }
    }

    class UDPClient
    {
        const int BUFSIZE = 512;

        int retval;

        // 데이터 통신에 사용할 변수
        byte[] buf;

        Socket sock;

        //로그 파일
        StreamWriter sw;

        IPEndPoint serveraddr;

        string token1 = "2019";
        string token2 = "2703";

        JObject jsonData;

        public UDPClient()
        {
            try
            {
                sw = File.AppendText(@"C:\Users\윤현후\source\repos\Socket\Client\ClientLog.txt");
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => " + $"{"CLIENT OPEN",-15}");
                sw.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                // 소켓 생성
                sock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            buf = new byte[BUFSIZE];
        }

        public void trySend(string IP,int PORT)
        {
            // 소켓 주소 객체 초기화
            serveraddr = new IPEndPoint(IPAddress.Parse(IP), PORT);

            //token  생성
            Random random = new Random();
            string[] randomNumber = new string[3];
            for (int i = 0; i < 3; i++)
            {
                randomNumber[i] = random.Next(1000, 10000).ToString();
            }
            string token = randomNumber[0] + '-' + token1 + '-' + randomNumber[1] + '-' + token2 + '-' + randomNumber[2];

            string tokenjson = "{\r\n    \"요청 TOKEN\" : \""+ token + "\"\r\n}";
            Console.WriteLine(tokenjson);

            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " => "
                 + $"{"SEND",-15} IP : {serveraddr.Address,-15} PORT : {serveraddr.Port,-10} TOKEN:{'\n'}{tokenjson}");
            sw.Flush();

            // 데이터 보내기 (최대 길이를 BUFSIZE로 제한)
            byte[] senddata = Encoding.Default.GetBytes(tokenjson);
            int size = senddata.Length;
            if (size > BUFSIZE) size = BUFSIZE;
            retval = sock.SendTo(senddata, 0, size,
                SocketFlags.None, serveraddr);
            Console.WriteLine(
                "[UDP 클라이언트] {0}바이트를 보냈습니다.", retval);
        }

        public void SendOK(int num)
        {
            for (int i = 0; i < num; i++)
            {
                string str = "ACK OK";
                // 데이터 보내기 (최대 길이를 BUFSIZE로 제한)
                byte[] senddata = Encoding.Default.GetBytes(str);
                int size = senddata.Length;
                if (size > BUFSIZE) size = BUFSIZE;
                retval = sock.SendTo(senddata, 0, size,
                    SocketFlags.None, serveraddr);
                Console.WriteLine(
                    "[UDP 클라이언트] {0}바이트를 보냈습니다.{1}", retval, str);

                Thread.Sleep(1000); // 1초 대기       
            }
        }

        public bool Write()
        {
            try
            {
                // 데이터 입력
                Console.Write("\n[보낼 데이터] ");
                string data = Console.ReadLine();
                if (data.Length == 0) return false;

                // 데이터 보내기 (최대 길이를 BUFSIZE로 제한)
                byte[] senddata = Encoding.Default.GetBytes(data);
                int size = senddata.Length;
                if (size > BUFSIZE) size = BUFSIZE;
                retval = sock.Send(senddata, 0, size, SocketFlags.None);
                Console.WriteLine(
                    "[TCP 클라이언트] {0}바이트를 보냈습니다.", retval);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public int Read()
        {
            try
            {
                // 데이터 받기
                IPEndPoint anyaddr = new IPEndPoint(IPAddress.Any, 0);
                EndPoint peeraddr = (EndPoint)anyaddr;
                retval = sock.ReceiveFrom(buf, BUFSIZE,
                    SocketFlags.None, ref peeraddr);

                // 받은 데이터 출력
                Console.WriteLine(
                    "[UDP 클라이언트] {0}바이트를 받았습니다.", retval);
                Console.WriteLine("[받은 데이터] {0}",
                    Encoding.Default.GetString(buf, 0, retval));

                string receiveToken = Encoding.Default.GetString(buf);

                //JSON 파싱
                jsonData = JObject.Parse(receiveToken);
                string jtoken = jsonData["응답 TOKEN"].ToString();
                string[] tokens = jtoken.Split('-');

                if (token2.Equals(tokens[0]) && token1.Equals(tokens[4]))
                    return 1;    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public void Close()
        {
            try
            {
                sw.Close();
                // 소켓 닫기
                sock.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            sw.Close();
        }
    }
}
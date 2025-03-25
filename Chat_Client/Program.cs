using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient();
            
            string? nikName = args[0];

            while (true)
            {
                Console.WriteLine("Введите сообщение");
                string? textToSend = Console.ReadLine();
                if (String.IsNullOrEmpty(textToSend))
                {
                    break;
                }

                Message msgToSend = new Message { UserName = nikName, MsgText = textToSend, MsgTime = DateTime.Now};

                string strToSend = msgToSend.ToJson();
                byte[] sendBuff = Encoding.UTF8.GetBytes(strToSend);

                udpClient.Send(sendBuff, localEP);

                try
                {
                    byte[] recievedBuff = udpClient.Receive(ref localEP);
                    string recievedStr = Encoding.UTF8.GetString(recievedBuff);

                    Message? recievedMsg = Message.FromJson(recievedStr);

                    if (recievedMsg != null)
                    {
                        Console.WriteLine(recievedMsg.ToString());
                    }
                    else
                    {
                        Console.WriteLine("Ошибка в чтении сообщения.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }
    }
}

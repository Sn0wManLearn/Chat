using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat_Client
{
    internal class Program
    {
        static IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        static UdpClient udpClient = new UdpClient();
        static void Main(string[] args)
        {

            string fromeName = args[0];

            Thread tr1 = new Thread(() => Sendler(fromeName));
            Thread tr2 = new Thread(() => Listener());

            try
            {
                tr1.Start();
                tr2.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Работа программы завершена.");
                return;
            }
        }

        public static void Sendler(string fromeName)
        {
            while (true)
            {
                Console.WriteLine("Введите имя получателя");
                string? sendToUser = Console.ReadLine();

                if (string.IsNullOrEmpty(sendToUser))
                {
                    Console.WriteLine("Имя пользователя не введено");
                    continue;
                }

                Console.WriteLine("Введите сообщение");
                string? textToSend = Console.ReadLine();

                if (String.IsNullOrEmpty(textToSend) && !string.IsNullOrWhiteSpace(textToSend))
                {
                    break;
                }

                Message msgToSend = new Message(fromeName, sendToUser, textToSend);

                string strToSend = msgToSend.ToJson();
                byte[] sendBuff = Encoding.UTF8.GetBytes(strToSend);
                udpClient.Send(sendBuff, localEP);

                byte[] receivedBuff = udpClient.Receive(ref localEP);
                string receivedStr = Encoding.UTF8.GetString(receivedBuff);

                Message? recievedMsg = Message.FromJson(receivedStr);

                if (recievedMsg != null)
                {
                    Console.WriteLine(recievedMsg.ToString());
                }
                else
                {
                    Console.WriteLine("Ошибка в чтении сообщения.");
                }



                if (textToSend.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
        }

        public static void Listener()
        {
            //IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            //UdpClient udpClient = new UdpClient();

            while (true)
            {
                byte[] receivedBuff = udpClient.Receive(ref localEP);
                string receivedStr = Encoding.UTF8.GetString(receivedBuff);
                Message? receivedMsg = Message.FromJson(receivedStr);

                if (receivedMsg != null)
                {
                    Console.WriteLine(receivedMsg.ToString());
                }
                else
                {
                    Console.WriteLine("Ошибка в чтении сообщения.");
                }
            }
        }
    }
}

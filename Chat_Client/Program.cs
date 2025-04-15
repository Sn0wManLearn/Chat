using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat_Client
{
    internal class Program
    {
        static IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        static UdpClient udpClient = new UdpClient(12346);
        static void Main(string[] args)
        {
            string fromName = "Alan"; //args[0];

            Task tskSend = new Task(() => Sendler(fromName));
            Task tskListen = new Task(() => Listener());

            tskSend.Start();
            tskListen.Start();

            Task.WaitAny(new Task[] { tskSend, tskListen });
        }
        static bool IsOkInput(string input)
        {
            return !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input);
        }

        static string GetInput()
        {
            string result = Console.ReadLine();
            while (!IsOkInput(result))
            {
                Console.WriteLine("Введена пустая строка. Попробуйте еще раз.");
                result = Console.ReadLine();
            }
            return result;
        }
        public static void Sendler(string fromName)
        {
            while (true)
            {
                try
                {                    
                    Console.WriteLine("Введите имя получателя");
                    string? sendToUser = GetInput();

                    Console.WriteLine("Введите сообщение");
                    string? textToSend = GetInput();

                    Message msgToSend = new Message(fromName, sendToUser, textToSend);

                    PrototypePattern ptp = new PrototypePattern(msgToSend);
                    Message msgWithConstTxt = (Message)ptp.Clone();
                    string strToSend = msgWithConstTxt.ToJson();

                    byte[] sendBuff = Encoding.UTF8.GetBytes(strToSend);
                    udpClient.Send(sendBuff, localEP);

                    if (textToSend.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Работа программы завершена.");
                }
            }
        }

        public static void Listener()
        {
            while (true)
            {
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Работа программы завершена.");
                }
            }
        }
    }
}

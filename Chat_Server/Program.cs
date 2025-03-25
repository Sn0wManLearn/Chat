using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Chat_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);

            UdpClient udpClient = new UdpClient(12345);

            Console.WriteLine("Сервер ожидает сообщение от клиента.");

            while (true)
            {
                try
                {
                    byte[] recieveBuff = udpClient.Receive(ref localEP);
                    string recievedStr = Encoding.UTF8.GetString(recieveBuff);
                    
                    Message? recievedMsg = Message.FromJson(recievedStr);

                    if (recievedMsg != null)
                    {
                        Console.WriteLine(recievedMsg.ToString());

                        Message sendMsg = new Message() { UserName = "Server", MsgText = "Сообщение получено", MsgTime = DateTime.Now };
                        string sendStr = sendMsg.ToJson();

                        byte[] sendBuff = Encoding.UTF8.GetBytes(sendStr);
                        udpClient.Send(sendBuff, localEP);
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

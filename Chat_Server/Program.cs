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

            bool run = true;

            while (run)
            {
                try
                {
                    byte[] recieveBuff = udpClient.Receive(ref localEP);
                    string recievedStr = Encoding.UTF8.GetString(recieveBuff);

                    Thread tr1 = new Thread(() =>
                    {
                        if (!string.IsNullOrEmpty(recievedStr))
                        {
                            Message? recievedMsg = Message.FromJson(recievedStr);
                            Console.WriteLine(recievedMsg?.ToString());

                            Message sendMsg = new Message() { UserName = "Server", MsgText = "Сообщение получено", MsgTime = DateTime.Now };
                            string sendStr = sendMsg.ToJson();

                            byte[] sendBuff = Encoding.UTF8.GetBytes(sendStr);
                            udpClient.Send(sendBuff, localEP);

                            if (recievedMsg.MsgText.Equals("exit", StringComparison.OrdinalIgnoreCase))
                            {
                                run = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ошибка в чтении сообщения.");
                        }
                    });                    

                    tr1.Start();
                    tr1.Join();
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}

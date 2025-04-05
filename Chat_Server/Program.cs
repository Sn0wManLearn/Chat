using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Chat_Server
{
    internal class Program
    {
        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken ct = cts.Token;
        static IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
        static UdpClient udpClient = new UdpClient(12345);
        static void Main()
        {
            Console.WriteLine("Сервер ожидает сообщение от клиента.");

            Task tsk1 = new Task(RecievingMSG, ct);
            Task tsk2 = new Task(() =>
            {
                Console.ReadKey();
                cts.Cancel();
            });

            tsk1.Start();
            tsk2.Start();

            while (true)
            {
                if (cts.IsCancellationRequested) return;
            }
        }

        static void RecievingMSG()
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    byte[] recieveBuff = udpClient.Receive(ref localEP);
                    string recievedStr = Encoding.UTF8.GetString(recieveBuff);

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
                            cts.Cancel();
                        }
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

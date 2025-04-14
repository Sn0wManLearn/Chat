using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Chat_Server
{
    internal class Program
    {
        static void Main()
        {
            UdpClient udpClient = new(12345); // Сокет для прослушивания входящих соединений
            IPEndPoint ep = new(IPAddress.Any, 0);

            string serverName = "Server";

            Console.WriteLine("Сервер ожидает сообщение от клиента.");

            Dictionary<String, IPEndPoint> clients = [];

            while (true)
            {
                var data = udpClient.Receive(ref ep);
                string msgStr = Encoding.UTF8.GetString(data); // Преобразование байтового массива в строку
                try
                {
                    Message msg = Message.FromJson(msgStr);
                    
                    Console.WriteLine(msg.ToString());

                    string replyMsg = "";

                    if (msg.ToName.ToLower() == "server")
                    {
                        if (msg.MsgText.ToLower() == "register")
                        {
                            clients.Add(msg.FromName, new IPEndPoint(ep.Address, ep.Port));
                            replyMsg = "Зарегистрирован";
                            Console.WriteLine($"Пользователь {msg.FromName} зарегистрирован.");
                        }
                        if (msg.MsgText.Equals("delete", StringComparison.OrdinalIgnoreCase))
                        {
                            clients.Remove(msg.FromName);
                            replyMsg = "Удален";
                            Console.WriteLine($"Пользователь {msg.FromName} удален.");
                        }
                        if (msg.MsgText.Equals("list", StringComparison.OrdinalIgnoreCase))
                        {
                            replyMsg = String.Join(", ", clients.Keys);
                            Console.WriteLine("Список пользователей: " + replyMsg);
                        }
                        else
                        {
                            replyMsg = "Неизвестная команда";
                        }
                    }
                    else
                    {
                        if (clients.TryGetValue(msg.ToName, out IPEndPoint client))
                        {
                            var forwardMessage = new Message(msg.FromName, msg.ToName, msg.MsgText).ToJson();
                            byte[] forvardBytes = System.Text.Encoding.UTF8.GetBytes(forwardMessage);
                            udpClient.Send(forvardBytes, forvardBytes.Length, ep);
                            Console.WriteLine("Сообщение отправлено" + msg.ToName);
                            replyMsg = "Отправлено";
                        }
                        else
                        {
                            replyMsg = "Пользователь не найден";
                        }
                    }

                    Message msgToReply = new Message(serverName, msg.FromName, replyMsg);

                    PrototypePattern ptp = new PrototypePattern(msgToReply);      // Prototype pattern
                    var replyMsgWithConstTxt = (Message)ptp.Clone();

                   
                    var replyMessageJson = replyMsgWithConstTxt.ToJson();

                    var replyBytes = Encoding.UTF8.GetBytes(replyMessageJson);
                    udpClient.Send(replyBytes, replyBytes.Length, ep);
                    Console.WriteLine("Ответ отправлен");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

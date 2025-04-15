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
            IPEndPoint localEP = new(IPAddress.Any, 0);

            string serverName = "Server";

            Console.WriteLine("Сервер ожидает сообщение от клиента.");

            Dictionary<String, IPEndPoint> clients = [];

            while (true)
            {
                byte[] data = udpClient.Receive(ref localEP);
                string msgStr = Encoding.UTF8.GetString(data); // Преобразование байтового массива в строку
                try
                {
                    Message msg = Message.FromJson(msgStr);
                    
                    Console.WriteLine(msg.ToString());
                    if (msg.MsgText.StartsWith("exit", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }

                    string replyMsg = "";

                    if (msg.ToName.ToLower() == "server")
                    {
                        if (msg.MsgText.StartsWith("register", StringComparison.InvariantCultureIgnoreCase))
                        {
                            clients.Add(msg.FromName, new IPEndPoint(localEP.Address, localEP.Port));
                            replyMsg = "Зарегистрирован";
                            Console.WriteLine($"Пользователь {msg.FromName} зарегистрирован.");
                        }
                        else if (msg.MsgText.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase))
                        {
                            clients.Remove(msg.FromName);
                            replyMsg = "Удален";
                            Console.WriteLine($"Пользователь {msg.FromName} удален.");
                        }
                        else if (msg.MsgText.StartsWith("list", StringComparison.InvariantCultureIgnoreCase))
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
                            udpClient.Send(forvardBytes, forvardBytes.Length, localEP);
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
                    udpClient.Send(replyBytes, replyBytes.Length, localEP);
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

using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Chat_Server
{
    internal class Program
    {
        //static private CancellationTokenSource cts = new CancellationTokenSource();
        //static private CancellationToken ct = cts.Token;

        //static IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
        //static UdpClient udpClient = new UdpClient(12345);

        //static Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        static void Main()
        {

            //Console.WriteLine("Сервер ожидает сообщение от клиента.");

            //Task tsk1 = new Task(RecievingMSG, ct);
            //Task tsk2 = new Task(() =>
            //{
            //    Console.ReadKey();
            //    cts.Cancel();
            //});

            //tsk1.Start();
            //tsk2.Start();

            //Task.WaitAny(new Task[] { tsk1, tsk2 });

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
                    PrototypePattern text = new PrototypePattern(msg);      // Prototype pattern
                    var msgWithConstantText = (Message)text.Clone();

                    Console.WriteLine(msgWithConstantText.ToString());
                    Console.WriteLine(msgWithConstantText.MsgText);

                    string replyMsg = "";

                    if (msgWithConstantText.ToName.ToLower() == "server")
                    {
                        if (msg.MsgText.Equals("register", StringComparison.OrdinalIgnoreCase))
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
                            var forwardMessage = new Message(msg.FromName, msg.ToName, msgWithConstantText.MsgText).ToJson();
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
                    PrototypePattern t = new PrototypePattern(msgToReply);   // Prototype pattern
                    var msgWithConstantTextToReply = (Message)t.Clone();

                    var replyMessageJson = msgWithConstantTextToReply.ToJson();

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

        static void RecievingMSG()
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    byte[] receiveBuff = udpClient.Receive(ref localEP);
                    string receivedStr = Encoding.UTF8.GetString(receiveBuff);

                    if (!string.IsNullOrEmpty(receivedStr) && !string.IsNullOrWhiteSpace(receivedStr))
                    {
                        Message? receivedMsg = Message.FromJson(receivedStr);

                        string replyMessage = "";

                        if (receivedMsg.ToName.ToLower() == "server")
                        {
                            if (receivedMsg.MsgText.ToLower() == "register")
                            {
                                clients.Add(receivedMsg.FromName, new IPEndPoint(localEP.Address, localEP.Port));
                                replyMessage = $"Пользователь {receivedMsg.FromName} зарегистрирован";
                                Console.WriteLine("Добавлен пользователь: " + receivedMsg.FromName);
                            }
                            else if (receivedMsg.MsgText.ToLower() == "delete")
                            {
                                clients.Remove(receivedMsg.FromName);
                                replyMessage = "Deleted";
                                Console.WriteLine($"Пользователь {receivedMsg.FromName} удален");
                            }
                            else if (receivedMsg.MsgText.ToLower() == "list")
                            {
                                replyMessage = String.Join(", ", clients.Keys);
                                Console.WriteLine("Список пользователей: " + replyMessage);
                            }
                            else
                            {
                                replyMessage = "Неизвестная команда";
                            }
                        }
                        else
                        {
                            if (clients.TryGetValue(receivedMsg.ToName, out IPEndPoint client))
                            {
                                var forwardMessage = new Message(receivedMsg.FromName, receivedMsg.ToName, receivedMsg.MsgText).ToJson();
                                byte[] forvardBytes = Encoding.UTF8.GetBytes(forwardMessage);
                                udpClient.Send(forvardBytes, forvardBytes.Length, localEP);
                                Console.WriteLine("Сообщение отправлено пользователю " + receivedMsg.ToName);
                                replyMessage = "Отправлено";
                            }
                            else
                            {
                                replyMessage = $"Пользователь {receivedMsg.ToName} не найден";
                            }
                        }
                        Console.WriteLine(receivedMsg?.ToString());

                        Message sendMsg = new Message("server", receivedMsg.ToName, replyMessage); 
                        string sendStr = sendMsg.ToJson();

                        byte[] sendBuff = Encoding.UTF8.GetBytes(sendStr);
                        udpClient.Send(sendBuff, localEP);

                        if (receivedMsg.MsgText.Equals("exit", StringComparison.OrdinalIgnoreCase))
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

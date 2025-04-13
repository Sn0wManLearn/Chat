using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    internal class SendMsgCommand : ICommand
    {
        private string _message;
        private IPEndPoint _endPoint;
        private DateTime _dateTime;
        private string _fromName;
        private string _toName;

        public SendMsgCommand(string message, IPEndPoint endPoint, DateTime dateTime, string fromName, string toName)
        {
            _message = message;
            _endPoint = endPoint;
            _dateTime = dateTime;
            _fromName = fromName;
            _toName = toName;
        }

        public void Execute()
        {
            UdpClient udpClient = new UdpClient(12345); // Сокет для отправки сообщения

            Message message = new Message(_fromName, _toName, _message, _dateTime);

            PrototypePattern text = new PrototypePattern(message);                      // Паттерн Prototype
            var messageWithConstantText = (Message)text.Clone();
            var messageJson = messageWithConstantText.ToJson();
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);
            udpClient.Send(messageBytes, messageBytes.Length, _endPoint);

            Console.WriteLine("Сообщение отправлено по UDP.");
        }
    }
}

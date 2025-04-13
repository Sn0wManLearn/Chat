using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat_Client
{
    internal class ReceiveMsgCommand : ICommand
    {
        private static UdpClient _udpClient;
        private IPEndPoint _endPoint;
        Message MessageReceived { get; set; }

        public ReceiveMsgCommand(IPEndPoint endPoint)
        {
            _udpClient = new UdpClient(12345); // Сокет для приема сообщений
            _endPoint = endPoint;
        }

        public void Execute()
        {
            byte[] data = _udpClient.Receive(ref _endPoint);
            string msgStr = System.Text.Encoding.UTF8.GetString(data); // Преобразование байтового массива в строку
            var msg = Message.FromJson(msgStr);
            var t = new PrototypePattern(msg);
            var msgWithConstText = (Message)t.Clone();
            Console.WriteLine(msgWithConstText.ToString());
            MessageReceived = msgWithConstText;
        }
    }
}

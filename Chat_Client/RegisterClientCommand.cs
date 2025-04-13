using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    internal class RegisterClientCommand : ICommand
    {
        private string _clientName;
        private IPEndPoint _endPoint;
        private static Dictionary<string, IPEndPoint> _clients;

        public RegisterClientCommand(string clientName, IPEndPoint endPoint)
        {
            _clientName = clientName;
            _endPoint = endPoint;
            _clients = new Dictionary<string, IPEndPoint>();
        }

        public void Execute()
        {
            _clients.Add(_clientName, _endPoint);
            Console.WriteLine($"{_clientName} зарегистрирован.");
        }
    }
}

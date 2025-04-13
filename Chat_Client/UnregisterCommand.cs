using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    internal class UnregisterCommand : ICommand
    {
        private Dictionary<string, IPEndPoint> _clients;
        private string _client;
        public UnregisterCommand(Dictionary<string, IPEndPoint> clients, string client)
        {
            _clients = clients;
            _client = client;
        }
        public void Execute()
        {
            _clients.Remove(_client);
            Console.WriteLine($"{_client} unregistered");
        }
    }
}

using Server.Net.IO;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    internal class Program
    {
        static List<Client> _users;
        static TcpListener _listener;

        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("172.22.237.81"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                //BroadcastConnection();
            }

        }
    }
}

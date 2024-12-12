

using Server.Net.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Server.Services;
using MySqlX.XDevAPI;

namespace Server
{

    public static class ClientManager
    {
        public static List<S_Client> ConnectedClients = new List<S_Client>();
    }

    internal class S_Program
    {
        static TcpListener _listener;

        static void Main(string[] args)
        {
            _listener = new TcpListener(IPAddress.Parse("172.24.226.173"), 7891);
            _listener.Start();
            Console.WriteLine("Server started... Waiting for connections.");

            while (true)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var client = new S_Client(tcpClient);

                Task.Run(() => HandleClient(client));
            }
        }

        static void HandleClient(S_Client client)
        {
            try
            {
                client.Process(CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {client.UID}: {ex.Message}");
            }
            finally
            {
                // Remove the client from the list when done
                ClientManager.ConnectedClients.Remove(client);
                Console.WriteLine($"Client {client.UID} disconnected.");
            }
        }


    }
}
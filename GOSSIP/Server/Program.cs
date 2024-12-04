using Server.Net.IO;
using System.Net.Sockets;
using System.Net;
﻿using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Server.Models;
using Server.Services;

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

			// example of using DB Services
            //using var db = new DatabaseService();

            //Console.WriteLine(
            //    JsonSerializer.Serialize(JsonSerializer.Deserialize<UserModel?>(JsonSerializer.Serialize(
            //        UsersService.SignIn(
            //    "email", "yurii.stelmakh.pz.2023@lpnu.ua", "password", db.Connection), 
            //        new JsonSerializerOptions
            //        {
            //            WriteIndented = true,
            //            ReferenceHandler = ReferenceHandler.Preserve 
            //        }
            //        ),
            //        new JsonSerializerOptions
            //        {
            //            ReferenceHandler = ReferenceHandler.Preserve
            //        }),
            //        new JsonSerializerOptions
            //        {
            //            WriteIndented = true,
            //            ReferenceHandler = ReferenceHandler.Preserve
            //        })
            //    );
        }
    }
}

//using Server.Net.IO;
//using System.Net.Sockets;
//using System.Net;
//﻿using System.Text.Json;
//using System.Text.Json.Serialization;
//using MySql.Data.MySqlClient;
//using Server.Models;
//using Server.Services;

//namespace Server
//{
//    internal class Program
//    {
//        static List<Client> _users;
//        static TcpListener _listener;

//        static void Main(string[] args)
//        {
//            _users = new List<Client>();
//            _listener = new TcpListener(IPAddress.Parse("172.22.237.81"), 7891);
//            _listener.Start();

//            while (true)
//            {
//                var client = new Client(_listener.AcceptTcpClient());
//                _users.Add(client);

//                //BroadcastConnection();
//            }

//			// example of using DB Services
//            //using var db = new DatabaseService();

//            //Console.WriteLine(
//            //    JsonSerializer.Serialize(JsonSerializer.Deserialize<UserModel?>(JsonSerializer.Serialize(
//            //        UsersService.SignIn(
//            //    "email", "yurii.stelmakh.pz.2023@lpnu.ua", "password", db.Connection), 
//            //        new JsonSerializerOptions
//            //        {
//            //            WriteIndented = true,
//            //            ReferenceHandler = ReferenceHandler.Preserve 
//            //        }
//            //        ),
//            //        new JsonSerializerOptions
//            //        {
//            //            ReferenceHandler = ReferenceHandler.Preserve
//            //        }),
//            //        new JsonSerializerOptions
//            //        {
//            //            WriteIndented = true,
//            //            ReferenceHandler = ReferenceHandler.Preserve
//            //        })
//            //    );
//        }
//    }
//}


using Server.Net.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Server.Models;
using Server.Services;
using MySqlX.XDevAPI;

namespace Server
{
    internal class S_Program
    {
        static List<S_Client> _users = new List<S_Client>();
        static TcpListener _listener;

        static void Main(string[] args)
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            Console.WriteLine("Server started... Waiting for connections.");

            while (true)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var client = new S_Client(tcpClient);
                _users.Add(client);
            }
        }

        //static bool AuthenticateClient(TcpClient tcpClient, out UserModel authenticatedUser)
        //{
        //    authenticatedUser = null;

        //    try
        //    {
        //        var networkStream = tcpClient.GetStream();
        //        var packetReader = new PacketReader(networkStream);

        //        var authUserPacket = packetReader.ReadPacket<AuthUserModel>();
        //        var authUserModel = authUserPacket.Data;

        //        using var db = new DatabaseService();

        //        var userModel = UsersService.SignIn("email", authUserModel.Username, authUserModel.Password, db.Connection);

        //        if (userModel != null)
        //        {
        //            // Authentication successful
        //            authenticatedUser = userModel;

        //            var packetBuilder = new PacketBuilder<UserModel>();
        //            var packetBytes = packetBuilder.GetPacketBytes(SignalsEnum.Login, userModel);

        //            networkStream.Write(packetBytes, 0, packetBytes.Length);
        //            networkStream.Flush();

        //            return true;
        //        }
        //        else
        //        {
        //            // Authentication failed
        //            var packetBuilder = new PacketBuilder<string>();
        //            var errorMessage = "Invalid username or password.";
        //            var packetBytes = packetBuilder.GetPacketBytes(SignalsEnum.AuthFailed, errorMessage);

        //            networkStream.Write(packetBytes, 0, packetBytes.Length);
        //            networkStream.Flush();

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error during authentication: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Server.Models;
using Server.Net.IO;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;


namespace Server
{
    public static class Globals
    {
        public static DatabaseService db = new DatabaseService();
    }

    public class S_Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public UserModel User { get; set; } = new UserModel { Username = "guest" };

        NetworkStream _networkStream;

        PacketReader _packetReader;

        public S_Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _networkStream = new NetworkStream(ClientSocket.Client);
            _packetReader = new PacketReader(_networkStream);

            Console.WriteLine($"{DateTime.Now} - Client {UID} {User.Username} connected :)");

            Task.Run(() => Process());
        }


        void Process()
        {
            while (true)
            {
                try
                {
                    var signal = _packetReader.ReadSignal();
                    Console.WriteLine($"[BEFORE] Signal {signal} received");

                    switch (signal)
                    {
                        case (byte)SignalsEnum.GetTopics:
                            {
                                List<TopicModel> allTopics = TopicsService.SelectAll(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetTopics, allTopics);
                                break;
                            }
                        case (byte)SignalsEnum.Login:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();

                                var authUserModel = _packetReader.DeserializePacket<AuthUserModel>(rawPacket);

                                using DatabaseService db = new DatabaseService();

                                var options = new JsonSerializerOptions
                                {
                                    WriteIndented = true,
                                    ReferenceHandler = ReferenceHandler.Preserve
                                };

                                var userModel = UsersService.SignIn("username", authUserModel.Username, authUserModel.Password, db.Connection);

                                if (userModel != null)
                                {
                                    // Serialize and deserialize the userModel to handle reference preservation
                                    var serializedUserModel = JsonSerializer.Serialize(
                                        JsonSerializer.Deserialize<UserModel?>(
                                            JsonSerializer.Serialize(userModel, options),
                                            new JsonSerializerOptions
                                            {
                                                ReferenceHandler = ReferenceHandler.Preserve
                                            }
                                        ),
                                        options
                                    );

                                    SendPacket(SignalsEnum.Login, serializedUserModel);
                                }
                                else
                                {
                                    // Handle failed login
                                }
                                break;
                            }
                        case (byte)SignalsEnum.Register:
                            {

                                var rawPacket = _packetReader.ReadRawPacket();

                                User = _packetReader.DeserializePacket<UserModel>(rawPacket);

                                //send userModel

                                Console.WriteLine($"{DateTime.Now} - Client {UID} {User.Username} registered).");
                                break;
                            }
                        case (byte)SignalsEnum.CreateTopic:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();

                                var newTopic = _packetReader.DeserializePacket<TopicModel>(rawPacket);

                                //add topic to bd

                                Console.WriteLine($"{DateTime.Now} - Client {UID} created topic: {newTopic.Title}");
                                break;
                            }
                        case (byte)SignalsEnum.EditTopic:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();


                                var editedTopic = _packetReader.DeserializePacket<TopicModel>(rawPacket);

                                //edit topic in bd


                                Console.WriteLine($"{DateTime.Now} - Client {UID} edited topic: {editedTopic.Title}");
                                break;
                            }
                        case (byte)SignalsEnum.DeleteTopic:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();

                                var deletedTopicId = _packetReader.DeserializePacket<int>(rawPacket);

                                //delete topic from bd

                                Console.WriteLine($"{DateTime.Now} - Client {UID} deleted topic with ID: {deletedTopicId}");
                                break;
                            }
                        case (byte)SignalsEnum.UpvoteTopic:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();


                                var upvotedTopicId = _packetReader.DeserializePacket<int>(rawPacket);


                                Console.WriteLine($"{DateTime.Now} - Client {UID} upvoted topic with ID: {upvotedTopicId}");
                                break;
                            }
                        case (byte)SignalsEnum.ReplyToTopic:
                            {
                                var rawPacket = _packetReader.ReadRawPacket();


                                var reply = _packetReader.DeserializePacket<ReplyModel>(rawPacket);
                                Console.WriteLine($"{DateTime.Now} - Client {UID} replied to topic: {reply.ID}");
                                break;
                            }
                        default:
                            //Console.WriteLine($"[SWITCH]Signal {signal} received");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} - Error: {ex.Message}");
                    ClientSocket.Close();
                    break;
                }
            }
        }
        #region Helpers

        private void SendPacket<T>(SignalsEnum signal, T user) where T : class
        {
            if (ClientSocket.Connected)
            {
                var authPacket = new PacketBuilder<T>();
                var packet = authPacket.GetPacketBytes(signal, user);
                ClientSocket.Client.Send(packet);
            }
        }

        private void SendPacket(SignalsEnum signal)
        {
            if (ClientSocket.Connected)
            {
                var authPacket = new PacketBuilder<object>();
                var packet = authPacket.GetPacketBytes(signal);
                ClientSocket.Client.Send(packet);
            }
        }

        #endregion

    }

}
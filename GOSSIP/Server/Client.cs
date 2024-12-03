using Server.Models;
using Server.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public UserModel user { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());

            var userPacket = _packetReader.ReadPacket<UserModel>();
            user = userPacket.Data;


            Console.WriteLine($"{DateTime.Now} - Client {UID} {user.Username} connected :)");

            Task.Run(() => Process());
        }


        // BD BD BD BD

        void Process()
        {
            while (true)
            {
                try
                {
                    var rawPacket = _packetReader.ReadRawPacket();
                    var signal = _packetReader.Signal;

                    switch (signal)
                    {
                        case (byte)SignalsEnum.Login:
                            var authUserModel = _packetReader.DeserializePacket<AuthUserModel>(rawPacket);

                            //go to bd and find our UserModel
                            //send userModel

                            user.Username = authUserModel.Username;
                            Console.WriteLine($"{DateTime.Now} - Client {UID} {user.Username} logged in with password {authUserModel.Password}).");
                            break;

                        case (byte)SignalsEnum.Register:
                            user = _packetReader.DeserializePacket<UserModel>(rawPacket);

                            //send userModel

                            Console.WriteLine($"{DateTime.Now} - Client {UID} {user.Username} registered).");
                            break;

                        case (byte)SignalsEnum.GetTopics:
                            var topics = _packetReader.DeserializePacket<List<TopicModel>>(rawPacket);

                            //renew topic, send topic, send fir 10 replies
                            Console.WriteLine($"{DateTime.Now} - Client {UID} received topics: {string.Join(", ", topics.Select(t => t.Title))}");
                            break;

                        case (byte)SignalsEnum.CreateTopic:
                            var newTopic = _packetReader.DeserializePacket<TopicModel>(rawPacket);

                            //add topic to bd

                            Console.WriteLine($"{DateTime.Now} - Client {UID} created topic: {newTopic.Title}");
                            break;

                        case (byte)SignalsEnum.EditTopic:
                            var editedTopic = _packetReader.DeserializePacket<TopicModel>(rawPacket);

                            //edit topic in bd


                            Console.WriteLine($"{DateTime.Now} - Client {UID} edited topic: {editedTopic.Title}");
                            break;

                        case (byte)SignalsEnum.DeleteTopic:
                            var deletedTopicId = _packetReader.DeserializePacket<int>(rawPacket);

                            //delete topic from bd

                            Console.WriteLine($"{DateTime.Now} - Client {UID} deleted topic with ID: {deletedTopicId}");
                            break;

                        case (byte)SignalsEnum.UpvoteTopic:
                            var upvotedTopicId = _packetReader.DeserializePacket<int>(rawPacket);


                            Console.WriteLine($"{DateTime.Now} - Client {UID} upvoted topic with ID: {upvotedTopicId}");
                            break;

                        case (byte)SignalsEnum.ReplyToTopic:
                            var reply = _packetReader.DeserializePacket<ReplyModel>(rawPacket);
                            Console.WriteLine($"{DateTime.Now} - Client {UID} replied to topic: {reply.ID}");
                            break;

                        default:
                            Console.WriteLine("Unknown signal");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{DateTime.Now} - Client {user.Username} disconnected :(");
                    ClientSocket.Close();
                    break;
                }
            }
        }

    }

}
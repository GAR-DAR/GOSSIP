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
    public static class Logging
    {
        public static void Log(string message, Guid guid, UserModel user)
        {
            Console.WriteLine($"{DateTime.Now} - Client {guid} {user.Username} {message} :)");
        }
        public static void LogRecived(SignalsEnum signal, Guid guid, UserModel user)
        {
            Console.WriteLine($"{DateTime.Now} [Recived] signal {(byte)signal} ({signal}) from user {guid} with name {user.Username}");
        }
        public static void LogSent(SignalsEnum signal, Guid guid, UserModel user)
        {
            Console.WriteLine($"{DateTime.Now} [Sent] signal {(byte)signal} ({signal}) for user {guid} with name {user.Username}");
        }
    }
    public class S_Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public UserModel User { get; set; } = new UserModel { Username = "guest" };
        public Mutex mutex = new Mutex();

        NetworkStream _networkStream;
        PacketReader _packetReader;
        private CancellationTokenSource _cancellationTokenSource;

        public S_Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _networkStream = new NetworkStream(ClientSocket.Client);
            _packetReader = new PacketReader(_networkStream);
            _cancellationTokenSource = new CancellationTokenSource();

            Logging.Log("connected", UID, User);

            Task.Run(() => Process(_cancellationTokenSource.Token));
        }


        void Process(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Logging.Log("process cancelled", UID, User);
                        break;
                    }

                    mutex.WaitOne();
                    var signal = _packetReader.ReadSignal();
                    mutex.ReleaseMutex();

                    if (signal == 255)
                    {
                        continue;
                    }

                    Logging.LogRecived((SignalsEnum)signal, UID, User);
                    switch (signal)
                    {
                        case (byte)SignalsEnum.Disconnect:
                            {
                                Logging.Log("disconnected", UID, User);
                                ClientSocket.Close();
                                _cancellationTokenSource.Cancel();
                                break;
                            }
                        case (byte)SignalsEnum.GetTopics:
                            {
                                List<TopicModel> allTopics = TopicsService.SelectAll(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetTopics, allTopics);
                                Logging.LogSent(SignalsEnum.GetTopics, UID, User);
                                break;
                            }
                        case (byte)SignalsEnum.Login:
                            {
                                mutex.WaitOne();
                                var authUserModel = _packetReader.ReadPacket<AuthUserModel>().Data;
                                mutex.ReleaseMutex();

                                UserModel userModel;
                                if(authUserModel.Username == null && authUserModel.Email != null)
                                {
                                    userModel = UsersService.SignIn("email", authUserModel.Email, authUserModel.Password, Globals.db.Connection);
                                    if (userModel == null)
                                    {
                                        Logging.Log("incorrect login or password", UID, User);
                                        SendPacket(SignalsEnum.LoginError);
                                        Logging.LogSent(SignalsEnum.LoginError, UID, User);
                                    }
                                    else
                                    {
                                        SendPacket(SignalsEnum.Login, userModel);
                                        Logging.LogSent(SignalsEnum.Login, UID, User);
                                    }
                                }
                                if (authUserModel.Username != null && authUserModel.Email == null)
                                {
                                    userModel = UsersService.SignIn("username", authUserModel.Username, authUserModel.Password, Globals.db.Connection);
                                    if (userModel == null)
                                    {
                                        Logging.Log("incorrect login or password", UID, User);
                                        SendPacket(SignalsEnum.LoginError);
                                        Logging.LogSent(SignalsEnum.LoginError, UID, User);
                                    }
                                    else
                                    {
                                        SendPacket(SignalsEnum.Login, userModel);
                                        Logging.LogSent(SignalsEnum.Login, UID, User);
                                    }
                                }
                                else
                                {
                                    Logging.Log("invalid login packet", UID, User);
                                }
                                break;

                            }
                            
                        case (byte)SignalsEnum.SignUp:
                            {
                                mutex.WaitOne();
                                var userModel = _packetReader.ReadPacket<UserModel>().Data;
                                mutex.ReleaseMutex();
                                User = userModel;

                                if (UsersService.SignUp(userModel, Globals.db.Connection))
                                {
                                    Logging.Log("registered", UID, User);
                                    SendPacket(SignalsEnum.SignUp, userModel);
                                    Logging.LogSent(SignalsEnum.SignUp, UID, User);
                                }
                                else
                                {
                                    Logging.Log("registration failed", UID, User);
                                    SendPacket(SignalsEnum.SignUpError);
                                    Logging.LogSent(SignalsEnum.SignUpError, UID, User);
                                }
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
                        default:
                            //Console.WriteLine($"[SWITCH]Signal {signal} received");
                            break;
                    }
                    mutex.WaitOne();
                        _packetReader.Signal = 255;
                        _packetReader.ClearStream();
                    mutex.ReleaseMutex();
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
                mutex.WaitOne();
                    ClientSocket.Client.Send(packet);
                mutex.ReleaseMutex();
            }
        }

        private void SendPacket(SignalsEnum signal)
        {
            if (ClientSocket.Connected)
            {
                var authPacket = new PacketBuilder<object>();
                var packet = authPacket.GetPacketBytes(signal);
                mutex.WaitOne();
                    ClientSocket.Client.Send(packet);
                mutex.ReleaseMutex();
            }
        }

        #endregion

    }

}
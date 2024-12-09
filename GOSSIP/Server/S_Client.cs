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

        public Dictionary<Guid, byte[]> unacknowledgedPackets = new Dictionary<Guid, byte[]>();

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
            while (!cancellationToken.IsCancellationRequested)
            {
                var signal = _packetReader.ReadSignal();
                Guid packetId = Guid.Empty;

                try
                {
                   
                    if (signal == 255)
                    {
                        continue;
                    }

                    Logging.LogRecived((SignalsEnum)signal, UID, User);

                    switch (signal)
                    {
                        
                        case (byte)SignalsEnum.Acknowledgement:
                        {
                                var ackPacket = _packetReader.ReadPacket<object>();
                                packetId = ackPacket.PacketId;
                                HandleAcknowledgement(packetId);
                                break;
                        }
                        case (byte)SignalsEnum.Disconnect:
                            {
                                Logging.Log("disconnected", UID, User);
                                ClientSocket.Close();
                                _cancellationTokenSource.Cancel();
                                break;
                            }
                        case (byte)SignalsEnum.GetTopics:
                            {
                                //List<TopicModel> allTopics = TopicsService.SelectAll(Globals.db.Connection);
                                List<TopicModel> allTopics = [];
                                SendPacket(SignalsEnum.GetTopics, allTopics);
                                Logging.LogSent(SignalsEnum.GetTopics, UID, User);
                                break;
                            }
                        case (byte)SignalsEnum.Login:
                            {
                                mutex.WaitOne();
                                var packet = _packetReader.ReadPacket<AuthUserModel>();
                                var authUserModel = packet.Data;
                                packetId = packet.PacketId;


                                mutex.ReleaseMutex();

                                UserModel userModel;
                                if(authUserModel.Username == null && authUserModel.Email != null)
                                {
                                    userModel = UsersService.SignIn(authUserModel.Email, null, authUserModel.Password, Globals.db.Connection);
                                    if (userModel == null)
                                    {
                                        Logging.Log("incorrect login or password", UID, User);
                                        SendPacket<object>(SignalsEnum.LoginError);
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
                                    userModel = UsersService.SignIn(null, authUserModel.Username ,authUserModel.Password, Globals.db.Connection);
                                    if (userModel == null)
                                    {
                                        Logging.Log("incorrect login or password", UID, User);
                                        SendPacket<object>(SignalsEnum.LoginError);
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

                                    var packet = _packetReader.ReadPacket<UserModel>();
                                    var userModel = packet.Data;
                                    packetId = packet.PacketId;

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
                                    SendPacket<object>(SignalsEnum.SignUpError);
                                    Logging.LogSent(SignalsEnum.SignUpError, UID, User);
                                }
                                break;
                            }
                        case (byte)SignalsEnum.RefreshUser:
                            {
                                mutex.WaitOne();

                                    var packet = _packetReader.ReadPacket<UserModel>();
                                    var userModel = packet.Data;
                                    packetId = packet.PacketId;
                        
                                mutex.ReleaseMutex();

                                User = UsersService.Select(userModel.ID, Globals.db.Connection);

                               // User.Status = "Student";

                                if (User != null) {
                                    Logging.Log("refreshing", UID, User);
                                    SendPacket(SignalsEnum.RefreshUser, User);
                                    Logging.LogSent(SignalsEnum.RefreshUser, UID, User);
                                }
                                else
                                {
                                    Logging.Log("User id is not found in db", UID, User);
                                }
                                
                                
                                break;
                            }
                        case (byte)SignalsEnum.ChangeUserPhoto:
                            {
                                mutex.WaitOne();

                                    var packet = _packetReader.ReadPacket<UserModel>();
                                    var userModel = packet.Data;
                                    packetId = packet.PacketId;

                                mutex.ReleaseMutex();

                                bool res = UsersService.ChangePhoto(userModel.ID, userModel.Photo, Globals.db.Connection);
                                if (res)
                                {
                                    Logging.Log("Change user photo", UID, User);
                                }
                                else
                                {
                                    Logging.Log("User id is not found in db", UID, User);
                                }

                                break;
                            }
                        
                        case (byte)SignalsEnum.CreateTopic:
                            {
                                mutex.WaitOne();

                                    var packet = _packetReader.ReadPacket<TopicModel>();
                                    var newTopic = packet.Data;
                                    packetId = packet.PacketId;

                                mutex.ReleaseMutex();

                                mutex.WaitOne();
                                    TopicsService.Insert(newTopic, Globals.db.Connection);
                                mutex.ReleaseMutex();

                                mutex.WaitOne();
                                    //var topics = TopicsService.SelectAll(Globals.db.Connection);
                                    List<TopicModel> topics = [];
                                mutex.ReleaseMutex();

                                Logging.Log("created topic", UID, User);
                                //TODO: [COMPLETED] надсилати список топіків, а не лише новий топік
                                SendPacket(SignalsEnum.GetTopics, topics);
                                Logging.LogSent(SignalsEnum.CreateTopic, UID, User);
                                break;
                            }
                        case (byte)SignalsEnum.SendMessage:
                            {
                                mutex.WaitOne();

                                    var packet = _packetReader.ReadPacket<MessageModel>();
                                    var message = packet.Data;
                                    packetId = packet.PacketId;

                                mutex.ReleaseMutex();

                                mutex.WaitOne();
                                MessagesService.Add(message, Globals.db.Connection);
                                mutex.ReleaseMutex();

                                Logging.Log("Message sent", UID, User);

                                //TODO: multicast



                                break;
                            }
                        case (byte)SignalsEnum.Logout:
                            {
                                Logging.Log("logged out", UID, User);
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
                finally
                {
                    SendAcknowledgement(packetId);
                }
            }
        }
        #region Helpers

        public void SendPacket<T>(SignalsEnum signal, T data = null) where T : class
        {
            var packetBuilder = new PacketBuilder<T>();
            var packetBytes = packetBuilder.GetPacketBytes(signal, data);
            var packetId = packetBuilder.PacketId;

            mutex.WaitOne();
            unacknowledgedPackets[packetId] = packetBytes;
            mutex.ReleaseMutex();

            ClientSocket.Client.Send(packetBytes);

            // Start acknowledgement timer
            StartAcknowledgementTimer(packetId);
        }

        private void StartAcknowledgementTimer(Guid packetId)
        {
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ =>
            {
                mutex.WaitOne();
                if (unacknowledgedPackets.ContainsKey(packetId))
                    {
                        // Resend packet
                        var packetBytes = unacknowledgedPackets[packetId];
                        ClientSocket.Client.Send(packetBytes);

                        // Restart acknowledgement timer
                        StartAcknowledgementTimer(packetId);
                    }
               mutex.ReleaseMutex();
            });
        }


        private void HandleAcknowledgement(Guid packetId)
        {
            mutex.WaitOne();
                if (unacknowledgedPackets.ContainsKey(packetId))
                {
                    unacknowledgedPackets.Remove(packetId);
                }
            mutex.ReleaseMutex();
        }

        public void SendAcknowledgement(Guid packetId)
        {
            var packetBuilder = new PacketBuilder<object>();
            var ackPacketBytes = packetBuilder.GetAcknowledgementPacketBytes(packetId);
            ClientSocket.Client.Send(ackPacketBytes);
        }

        #endregion

    }

}
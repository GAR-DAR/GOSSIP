using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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
using System.ComponentModel.DataAnnotations;

namespace Server
{
    public static class Globals
    {
        public static DatabaseService db = new DatabaseService();
    }

    public static class Logging
    {
        public static void Log(string message, Guid guid, UserModelID user)
        {
            Console.WriteLine($"{DateTime.Now} - Client {guid} {user.Username} {message} :)");
        }
        public static void LogRecived(SignalsEnum signal, Guid guid, UserModelID user)
        {
            Console.WriteLine($"{DateTime.Now} [Recived] signal {(byte)signal} ({signal}) from user {guid} with name {user.Username}");
        }
        public static void LogSent(SignalsEnum signal, Guid guid, UserModelID user)
        {
            Console.WriteLine($"{DateTime.Now} [Sent] signal {(byte)signal} ({signal}) for user {guid} with name {user.Username}");
        }
    }
    
    public class S_Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public UserModelID User { get; set; } = new UserModelID { Username = "guest" };
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

            // Add the client to the list of connected clients
            ClientManager.ConnectedClients.Add(this);

            Logging.Log("connected", UID, User);

        }


        public void Process(CancellationToken cancellationToken)
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
                        case (byte)SignalsEnum.GetAllUsers:
                            {
                                
                                mutex.WaitOne();
                                List<UserModelID> allUsers = UsersService.SelectAll(Globals.db.Connection);
                                
                                SendPacket(SignalsEnum.GetAllUsers, allUsers);
                                Logging.LogSent(SignalsEnum.GetAllUsers, UID, User);

                                mutex.ReleaseMutex();

                                break;
                            }

                        case (byte)SignalsEnum.GetTopics:
                            {
                                mutex.WaitOne();
                                List<TopicModelID> allTopics = TopicsService.SelectAll(Globals.db.Connection);
                                
                                SendPacket(SignalsEnum.GetTopics, allTopics);

                                Logging.LogSent(SignalsEnum.GetTopics, UID, User);

                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.Disconnect:
                            {
                                mutex.WaitOne();
                                Logging.Log("disconnected", UID, User);
                                ClientSocket.Close();
                                _cancellationTokenSource.Cancel();

                                // Remove the client from the list of connected clients
                                ClientManager.ConnectedClients.Remove(this);
                                mutex.ReleaseMutex();
                                break;
                            }
                        
                        case (byte)SignalsEnum.Login:
                            {

                                mutex.WaitOne();
                                var authUserModel = _packetReader.ReadPacket<AuthUserModelID>().Data;
                                
                                UserModelID userModel;
                                if (authUserModel.Username == null && authUserModel.Email != null)
                                {
                                    
                                    userModel = UsersService.SignIn(authUserModel.Email, null, authUserModel.Password, Globals.db.Connection);
                                    
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
                                    
                                    userModel = UsersService.SignIn(null, authUserModel.Username, authUserModel.Password, Globals.db.Connection);
                                    
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
                                mutex.ReleaseMutex();
                                break;

                            }

                        case (byte)SignalsEnum.SignUp:
                            {
                                mutex.WaitOne();
                                var userModel = _packetReader.ReadPacket<UserModelID>().Data;
                                
                                var temp = UsersService.SignUp(userModel, Globals.db.Connection);
                                
                                if (temp != null)  //no error if Andriy
                                {
                                    User = temp;
                                    Logging.Log("registered", UID, User);
                                    SendPacket(SignalsEnum.SignUp, temp);
                                    Logging.LogSent(SignalsEnum.SignUp, UID, User);
                                }
                                else
                                {
                                    Logging.Log("registration failed", UID, User);
                                    SendPacket(SignalsEnum.SignUpError);
                                    Logging.LogSent(SignalsEnum.SignUpError, UID, User);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.Logout:
                            {
                                mutex.WaitOne();
                                Logging.Log("logged out", UID, User);
                                SendPacket(SignalsEnum.Logout);
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.RefreshUser:
                            {
                                mutex.WaitOne();
                                var userModel = _packetReader.ReadPacket<UserModelID>().Data;
                                

                                User = UsersService.SelectById(userModel.ID, Globals.db.Connection);

                                if (User != null) {
                                    Logging.Log("refreshing", UID, User);
                                    SendPacket(SignalsEnum.RefreshUser, User);
                                    Logging.LogSent(SignalsEnum.RefreshUser, UID, User);
                                }
                                else
                                {
                                    Logging.Log("User id is not found in db", UID, User);
                                }
                                mutex.ReleaseMutex();

                                break;
                            }

                        
                        case (byte)SignalsEnum.ChangeUserPhoto: //will fix when Andriy will fix db
                            {
                                mutex.WaitOne();
                                var userModelID = _packetReader.ReadPacket<UserModelID>().Data;
                                
                                bool res = UsersService.ChangePhoto(userModelID.ID, userModelID.Photo, Globals.db.Connection);
                                if (res)
                                {
                                    Logging.Log("Change user photo", UID, User);
                                }
                                else
                                {
                                    Logging.Log("User id is not found in db", UID, User);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                       
                        case (byte)SignalsEnum.CreateTopic:
                            {
                                mutex.WaitOne();
                                var newTopic = _packetReader.ReadPacket<TopicModelID>().Data;
                                
                                TopicsService.Insert(newTopic, Globals.db.Connection);
                                
                                var topics = TopicsService.SelectAll(Globals.db.Connection);
                                
                                Logging.Log("created topic", UID, User);
                                SendPacket(SignalsEnum.GetTopics, topics);
                                Logging.LogSent(SignalsEnum.CreateTopic, UID, User);

                                mutex.ReleaseMutex();
                                break;
                            }

                            case (byte)SignalsEnum.GetReplies:
                            {
                                mutex.WaitOne();
                                var topicId = _packetReader.ReadPacket<uint>().Data;

                                var parentReplies = TopicsService.SelectParentRepliesByTopic(topicId, Globals.db.Connection);
                                SendPacket(SignalsEnum.GetParentReplies, parentReplies);

                                var childReplies = TopicsService.SelectChildRepliesByTopic(topicId, Globals.db.Connection);
                                SendPacket(SignalsEnum.GetChildReplies, childReplies);

                                

                                Logging.LogSent(SignalsEnum.GetReplies, UID, User);
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.CreateReply:
                            {
                                mutex.WaitOne();
                                var reply = _packetReader.ReadPacket<ParentReplyModelID>().Data;
                               

                                RepliesService.Add(reply, Globals.db.Connection);
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.CreateReplyToReply:
                            {
                                mutex.WaitOne();
                                var reply = _packetReader.ReadPacket<ParentReplyModelID>().Data;
                                
                                RepliesService.Add(reply, Globals.db.Connection);


                                SendPacket(SignalsEnum.CreateReplyToReply, reply);
                                mutex.ReleaseMutex();
                                break;
                            }

                       
                        case (byte)SignalsEnum.GetUserChats:
                            {
                                mutex.WaitOne();
                                var id = _packetReader.ReadPacket<uint>().Data;
                                
                                var chats = ChatsService.SelectChatsByUser(id, Globals.db.Connection);
                               
                                SendPacket(SignalsEnum.GetUserChats, chats);

                                Logging.LogSent(SignalsEnum.GetUserChats, UID, User);

                                mutex.ReleaseMutex();

                                break;
                            }

                        case (byte)SignalsEnum.SendMessage:
                            {
                                mutex.WaitOne();
                                var message = _packetReader.ReadPacket<MessageModelID>().Data;
                                
                                MessagesService.Add(message, Globals.db.Connection);
                                
                                Logging.Log("Message sent", UID, User);

                                //TODO: multicast

                                
                                var chatUsers = ChatsService.SelectUsersByChat(message.ChatID, Globals.db.Connection);
                               
                                // Multicast the message to all connected clients
                                foreach (var client in ClientManager.ConnectedClients)
                                {
                                    if (client.UID != this.UID) // Avoid sending the message to the sender
                                    {
                                        client.SendPacket(SignalsEnum.MessageMulticast, message);
                                    }
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.GetAllUsersMessage:
                            {
                                mutex.WaitOne(); 
                                var ids = _packetReader.ReadPacket<List<uint>>().Data; //should be list<chatid>
                                
                                List<MessageModelID> messages = [];
 
                                foreach(var id in ids)
                                {
                                    messages.AddRange(ChatsService.SelectMessagesByChat(id, Globals.db.Connection));
                                }

                                SendPacket(SignalsEnum.GetAllUsersMessage, messages);
                                Logging.LogSent(SignalsEnum.GetAllUsersMessage, UID, User);
                                mutex.ReleaseMutex();
                                break;
                            }
                       

                        case (byte)SignalsEnum.StartChat:
                            {
                                mutex.WaitOne();

                                var chat = _packetReader.ReadPacket<ChatModelID>().Data;

                                ChatsService.Create(chat, Globals.db.Connection);

                               

                                Logging.Log("Chat created", UID, User);
                                mutex.ReleaseMutex();


                                break;
                            }
                        
                        case (byte)SignalsEnum.GetStatuses:
                            {
                                mutex.WaitOne();
                                List<string> statuses = UsersService.GetStatuses(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetStatuses, statuses);
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetFieldsOfStudy:
                            {
                                mutex.WaitOne();
                                List<string> fields = UsersService.GetFieldsOfStudy(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetFieldsOfStudy, fields);
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetSpecializations:
                            {
                                mutex.WaitOne();
                                List<string> specializations = UsersService.GetSpecializations(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetSpecializations, specializations);
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetUniversities:
                            {
                                mutex.WaitOne();
                                List<string> universities = UsersService.GetUniversities(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetUniversities, universities);
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetDegrees:
                            {
                                mutex.WaitOne();
                                List<string> degrees = UsersService.GetDegrees(Globals.db.Connection);
                                SendPacket(SignalsEnum.GetDegrees, degrees);
                                mutex.ReleaseMutex();
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

        private void SendReplyPacket<T>(SignalsEnum signal, T data) where T : class
        {
            if (ClientSocket.Connected)
            {
                var authPacket = new PacketBuilder<T>();
                var packet = authPacket.GetReplyPacketBytes(signal, data);
                mutex.WaitOne();
                ClientSocket.Client.Send(packet);
                mutex.ReleaseMutex();
            }
        }

        private void SendPacket<T>(SignalsEnum signal, T data) where T : class
        {
            if (ClientSocket.Connected)
            {
                var authPacket = new PacketBuilder<T>();
                var packet = authPacket.GetPacketBytes(signal, data);
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
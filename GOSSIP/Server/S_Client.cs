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
using Google.Protobuf.WellKnownTypes;

namespace Server
{
    public static class Globals
    {
        public static DatabaseService db = new DatabaseService();
    }

    public static class Logging
    {
        public static void Log(string message, Guid guid, UserModelID user, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now} - Client {guid} {user.Username} {message} :)");
        }
        public static void LogRecived(SignalsEnum signal, Guid guid, UserModelID user, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now} [RecieWed] signal {(byte)signal} ({signal}) from user {guid} with name {user.Username}");
        }
        public static void LogSent(SignalsEnum signal, Guid guid, UserModelID user, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now} [Sented] signal {(byte)signal} ({signal}) for user {guid} with name {user.Username}");
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

            Logging.Log("connected", UID, User, ConsoleColor.Green);

        }


        public void Process(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Logging.Log("process cancelled", UID, User, ConsoleColor.Red);
                        break;
                    }

                    mutex.WaitOne();
                    var signal = _packetReader.ReadSignal();
                    mutex.ReleaseMutex();

                    if (signal == 255)
                    {
                        continue;
                    }

                    Logging.LogRecived((SignalsEnum)signal, UID, User, ConsoleColor.Green);

                    switch (signal)
                    {
                        case (byte)SignalsEnum.GetAllUsers:
                            {
                                mutex.WaitOne();
                                List<UserModelID> allUsers = UsersService.SelectAll(Globals.db.Connection);

                                if (allUsers != null)
                                {
                                    SendPacket(SignalsEnum.GetAllUsers, allUsers);
                                    Logging.LogSent(SignalsEnum.GetAllUsers, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("No users in database?", UID, User, ConsoleColor.Yellow);
                                }

                                mutex.ReleaseMutex();

                                break;
                            }

                        case (byte)SignalsEnum.GetTopics:
                            {
                                mutex.WaitOne();
                                List<TopicModelID> allTopics = TopicsService.SelectAll(Globals.db.Connection);

                                if(allTopics != null)
                                {
                                    SendPacket(SignalsEnum.GetTopics, allTopics);
                                    Logging.LogSent(SignalsEnum.GetTopics, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("No topics in database?", UID, User, ConsoleColor.Yellow);
                                }

                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.Disconnect:
                            {
                                mutex.WaitOne();
                                Logging.Log("disconnected", UID, User, ConsoleColor.Red);
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
                                        Logging.Log("incorrect login or password", UID, User, ConsoleColor.Red);
                                        SendPacket(SignalsEnum.LoginError);
                                        Logging.LogSent(SignalsEnum.LoginError, UID, User, ConsoleColor.Red);
                                    }
                                    else
                                    {
                                        SendPacket(SignalsEnum.Login, userModel);
                                        Logging.LogSent(SignalsEnum.Login, UID, User, ConsoleColor.Blue);
                                    }
                                }
                                if (authUserModel.Username != null && authUserModel.Email == null)
                                {
                                    userModel = UsersService.SignIn(null, authUserModel.Username, authUserModel.Password, Globals.db.Connection);

                                    if (userModel == null)
                                    {
                                        Logging.Log("incorrect login or password", UID, User, ConsoleColor.Red);
                                        SendPacket(SignalsEnum.LoginError);
                                        Logging.LogSent(SignalsEnum.LoginError, UID, User, ConsoleColor.Red);
                                    }
                                    else
                                    {
                                        User = userModel;
                                        SendPacket(SignalsEnum.Login, userModel);
                                        Logging.LogSent(SignalsEnum.Login, UID, User, ConsoleColor.Blue);
                                    }
                                }
                                else
                                {
                                    Logging.Log("invalid login packet", UID, User, ConsoleColor.Red);
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
                                    Logging.Log("registered", UID, User, ConsoleColor.Green);
                                    SendPacket(SignalsEnum.SignUp, temp);
                                    Logging.LogSent(SignalsEnum.SignUp, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("registration failed", UID, User, ConsoleColor.Red);
                                    SendPacket(SignalsEnum.SignUpError);
                                    Logging.LogSent(SignalsEnum.SignUpError, UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.Logout:
                            {
                                mutex.WaitOne();
                                Logging.Log("logged out", UID, User, ConsoleColor.Green);
                                User = new UserModelID { Username = "guest"};
                                SendPacket(SignalsEnum.Logout);

                                Logging.LogSent(SignalsEnum.Logout, UID, User, ConsoleColor.Blue);

                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.Refresh:
                            {
                                mutex.WaitOne();
                                
                                var topics = TopicsService.SelectAll(Globals.db.Connection);
                                var users = UsersService.SelectAll(Globals.db.Connection);

                                if (topics != null && users != null)
                                {
                                    SendPacket(SignalsEnum.GetTopics, topics);
                                    Logging.LogSent(SignalsEnum.GetTopics, UID, User, ConsoleColor.Blue);

                                    SendPacket(SignalsEnum.GetAllUsers, users);
                                    Logging.LogSent(SignalsEnum.GetAllUsers, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.LogSent(SignalsEnum.GetTopics, UID, User, ConsoleColor.Red);
                                    Logging.LogSent(SignalsEnum.GetAllUsers, UID, User, ConsoleColor.Red);
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
                                    Logging.Log("Changed user photo", UID, User, ConsoleColor.Green);
                                }
                                else
                                {
                                    Logging.Log("User id is not found in db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.CreateTopic:
                            {
                                mutex.WaitOne();
                                var newTopic = _packetReader.ReadPacket<TopicModelID>().Data;

                                var isCorrect = TopicsService.Insert(newTopic, Globals.db.Connection);

                                if (isCorrect != null)
                                {
                                    var topics = TopicsService.SelectAll(Globals.db.Connection);

                                    Logging.Log("created topic", UID, User, ConsoleColor.Green);
                                    SendPacket(SignalsEnum.GetTopics, topics);
                                    Logging.LogSent(SignalsEnum.CreateTopic, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error creating topic", UID, User, ConsoleColor.Red);

                                }

                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.GetReplies:
                            {
                                mutex.WaitOne();
                                var topicId = _packetReader.ReadPacket<uint>().Data;

                                
                                var topic = TopicsService.SelectById(topicId, Globals.db.Connection);
                                if (topic != null)
                                {
                                    SendPacket(SignalsEnum.GetReplies, topic);
                                    Logging.LogSent(SignalsEnum.GetReplies, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error selecting replies by topic id", UID, User, ConsoleColor.Red);
                                }

                                var parentReplies = TopicsService.SelectParentRepliesByTopic(topicId, Globals.db.Connection);
                                if (parentReplies != null)
                                {
                                    SendPacket(SignalsEnum.GetParentReplies, parentReplies);
                                    Logging.LogSent(SignalsEnum.GetParentReplies, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error selecting parent replies by topic id", UID, User, ConsoleColor.Red);
                                }

                                var childReplies = TopicsService.SelectChildRepliesByTopic(topicId, Globals.db.Connection);
                                if (childReplies != null)
                                {
                                    SendPacket(SignalsEnum.GetChildReplies, childReplies);
                                    Logging.LogSent(SignalsEnum.GetChildReplies, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error selecting child replies by topic id", UID, User, ConsoleColor.Red);
                                }

                                //Logging.LogSent(SignalsEnum.GetReplies, UID, User, ConsoleColor.Blue);
                                mutex.ReleaseMutex();
                                break;
                            }


                        case (byte)SignalsEnum.CreateReply:
                            {
                                mutex.WaitOne();
                                var replyID = _packetReader.ReadPacket<ParentReplyModelID>().Data;

                                var reply = RepliesService.Add(replyID, Globals.db.Connection);
                                if (reply != null)
                                {
                                    SendPacket(SignalsEnum.CreateReply, replyID);
                                    Logging.LogSent(SignalsEnum.CreateReply, UID, User, ConsoleColor.Blue);
                                }
                                else
                                    Logging.Log("Error creating reply", UID, User, ConsoleColor.Red);

                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.CreateReplyToReply:
                            {
                                mutex.WaitOne();
                                var childReply = _packetReader.ReadPacket<ChildReplyModelID>().Data;

                                var childReplyModelID = RepliesService.AddChild(childReply, Globals.db.Connection);

                                if (childReplyModelID != null)
                                {
                                    SendPacket(SignalsEnum.CreateReplyToReply, childReplyModelID);
                                    Logging.LogSent(SignalsEnum.CreateReplyToReply, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error creating reply to reply", UID, User, ConsoleColor.Red);
                                }

                                mutex.ReleaseMutex();
                                break;
                            }


                        case (byte)SignalsEnum.GetUserChats:
                            {
                                mutex.WaitOne();
                                var id = _packetReader.ReadPacket<uint>().Data;

                                var chats = ChatsService.SelectChatsByUser(id, Globals.db.Connection);
                                if (chats != null)
                                {
                                    SendPacket(SignalsEnum.GetUserChats, chats);
                                    Logging.LogSent(SignalsEnum.GetUserChats, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("No chats in database?", UID, User, ConsoleColor.Red);
                                }

                                mutex.ReleaseMutex();

                                break;
                            }

                        case (byte)SignalsEnum.SendMessage:
                            {
                                mutex.WaitOne();
                                var message = _packetReader.ReadPacket<MessageModelID>().Data;

                                var isCorrect = MessagesService.Add(message, Globals.db.Connection);
                                if (isCorrect != null)
                                {
                                    Logging.Log("Message sent", UID, User, ConsoleColor.Green);
                                }
                                else
                                {;
                                    Logging.Log("Error durring adding message", UID, User, ConsoleColor.Red);
                                }

                                var chatUsers = ChatsService.SelectUsersByChat(message.ChatID, Globals.db.Connection);

                                // Multicast the message to all connected clients
                                foreach (var client in ClientManager.ConnectedClients)
                                {
                                    if (client.UID != this.UID) // Avoid sending the message to the sender
                                    {
                                        client.SendPacket(SignalsEnum.MessageMulticast, message);
                                        Logging.LogSent(SignalsEnum.MessageMulticast, UID, User, ConsoleColor.Blue);
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

                                foreach (var id in ids)
                                {
                                    messages.AddRange(ChatsService.SelectMessagesByChat(id, Globals.db.Connection));
                                }

                                if(messages != null)
                                {
                                    SendPacket(SignalsEnum.GetAllUsersMessage, messages);
                                    Logging.LogSent(SignalsEnum.GetAllUsersMessage, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("No messages from user?", UID, User, ConsoleColor.Red);
                                }

                                mutex.ReleaseMutex();
                                break;
                            }


                        case (byte)SignalsEnum.StartChat:
                            {
                                mutex.WaitOne();
                                var chat = _packetReader.ReadPacket<ChatModelID>().Data;

                                var isCorrect = ChatsService.Create(chat, Globals.db.Connection);

                                if (isCorrect != null)
                                {
                                    Logging.Log("Chat created", UID, User, ConsoleColor.Green);
                                }

                                var chats = ChatsService.SelectChatsByUser(chat.UserIDs[0], Globals.db.Connection);
                                if (chats != null)
                                {
                                    SendPacket(SignalsEnum.GetUserChats, chats);
                                    Logging.LogSent(SignalsEnum.GetUserChats, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("No chats from user?", UID, User, ConsoleColor.Red);
                                }
                                
                                foreach (var client in ClientManager.ConnectedClients)
                                {
                                    if (chat.UserIDs.Contains(client.User.ID))
                                    {
                                        client.SendPacket(SignalsEnum.GetUserChats, chats);
                                        Logging.LogSent(SignalsEnum.GetUserChats, UID, User, ConsoleColor.Blue);
                                    }
                                }

                                mutex.ReleaseMutex();

                                break;
                            }

                        case (byte)SignalsEnum.BanUser:
                            {
                                mutex.WaitOne();

                                var id = _packetReader.ReadPacket<uint>().Data;

                                if(UsersService.Ban(id, Globals.db.Connection))
                                {
                                    var temp = UsersService.SelectById(id, Globals.db.Connection);

                                    SendPacket(SignalsEnum.BanUser, temp);

                                    Logging.Log($"moderator baned {temp.Username} ))))) ", UID, User, ConsoleColor.DarkMagenta);
                                }
                                else
                                {
                                    Logging.Log("Ban error", UID, User, ConsoleColor.Red);
                                }

                                

                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.GetStatuses:
                            {
                                mutex.WaitOne();
                                List<string> statuses = UsersService.GetStatuses(Globals.db.Connection);
                                if (statuses != null)
                                {
                                    SendPacket(SignalsEnum.GetStatuses, statuses);
                                    Logging.LogSent(SignalsEnum.GetStatuses, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during getting statuses from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.GetBannedUsers:
                            {
                                mutex.WaitOne();
                                List<UserModelID> bannedUsers = UsersService.SelectBannedUsers(Globals.db.Connection);
                                if (bannedUsers != null)
                                {
                                    SendPacket(SignalsEnum.GetBannedUsers, bannedUsers);
                                    Logging.LogSent(SignalsEnum.GetBannedUsers, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during getting banned users from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }

                        case (byte)SignalsEnum.UnbanUser:
                            {
                                mutex.WaitOne();
                                var id = _packetReader.ReadPacket<uint>().Data;
                                if (id != null)
                                {
                                    if (UsersService.Unban(id, Globals.db.Connection))
                                    {
                                        var users = UsersService.SelectAll(Globals.db.Connection);
                                        SendPacket(SignalsEnum.GetAllUsers, users);
                                    }
                                }
                                else
                                {
                                    Logging.Log("Error during unbanning user", UID, User, ConsoleColor.Red);
                                }
                                break;
                            }
                        case (byte)SignalsEnum.GetFieldsOfStudy:
                            {
                                mutex.WaitOne();
                                List<string> fields = UsersService.GetFieldsOfStudy(Globals.db.Connection);
                                if (fields != null)
                                {
                                    SendPacket(SignalsEnum.GetFieldsOfStudy, fields);
                                    Logging.LogSent(SignalsEnum.GetFieldsOfStudy, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during getting fields from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetSpecializations:
                            {
                                mutex.WaitOne();
                                List<string> specializations = UsersService.GetSpecializations(Globals.db.Connection);
                                if (specializations != null)
                                {
                                    SendPacket(SignalsEnum.GetSpecializations, specializations);
                                    Logging.LogSent(SignalsEnum.GetSpecializations, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during specializations fields from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetUniversities:
                            {
                                mutex.WaitOne();
                                List<string> universities = UsersService.GetUniversities(Globals.db.Connection);
                                if (universities != null)
                                {
                                    SendPacket(SignalsEnum.GetUniversities, universities);
                                    Logging.LogSent(SignalsEnum.GetUniversities, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during universities fields from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.GetDegrees:
                            {
                                mutex.WaitOne();
                                List<string> degrees = UsersService.GetDegrees(Globals.db.Connection);
                                if (degrees != null)
                                {
                                    SendPacket(SignalsEnum.GetDegrees, degrees);
                                    Logging.LogSent(SignalsEnum.GetDegrees, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during degrees degrees from db", UID, User, ConsoleColor.Red);
                                }
                                mutex.ReleaseMutex();
                                break;
                            }
                        case (byte)SignalsEnum.EditUser:
                            {
                                mutex.WaitOne();
                                var userModelID = _packetReader.ReadPacket<UserModelID>().Data;

                                UsersService.ChangeInfo(userModelID, Globals.db.Connection);
                                var newUserModelID = UsersService.SelectById(userModelID.ID, Globals.db.Connection);

                                if(newUserModelID != null)
                                {
                                    SendPacket(SignalsEnum.EditUser, newUserModelID);
                                    Logging.LogSent(SignalsEnum.EditUser, UID, User, ConsoleColor.Blue);
                                }
                                else
                                {
                                    Logging.Log("Error during editing user info", UID, User, ConsoleColor.Red);
                                }

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
                    Logging.Log($"Error: {ex.Message}", UID, User, ConsoleColor.Red);
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
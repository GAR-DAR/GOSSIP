using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using GOSSIP.Net.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Diagnostics;
using GOSSIP;
using GOSSIP.ViewModels;
using System.Windows.Controls.Primitives;

namespace GOSSIP.Net
{
    public static class Globals
    {
        public static Server server = new Server();

        public static UserModel User_Cache { get; set; }
        public static List<UserModel> AllUsers_Cache { get; set; } = [];
        public static List<TopicModel> Topics_Cache { get; set; } = [];
        public static TopicModel OpenedTopic_Cache { get; set; }


        public class Server
        {
            TcpClient _client;

            public PacketReader packetReader;

            private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

            public Server()
            {
                _client = new TcpClient();
            }

            //evens gets from server

            #region Events
            public event Action connectedEvent;
            public event Action userDisonnectedEvent;

            public event Action<UserModel> signUpEvent;
            public event Action<UserModel> editUserEvent;
            public event Action logoutEvent;
            public event Action<UserModel> loginEvent;
            public event Action<List<TopicModel>> getTopicsEvent;
            public event Action<List<UserModel>> getBannedUsersEvent;

            public event Action<List<string>> getStatusesEvent;
            public event Action<List<string>> getFieldOfStudyEvent;
            public event Action<List<string>> getSpecializationsEvent;
            public event Action<List<string>> getUniversitiesEvent;
            public event Action<List<string>> getDegreesEvent;
            public event Action<TopicModel> openTopicEvent;
            public event Action<List<UserModel>> getUsersEvent;
            public event Action<object> openChatsEvent;

            public event Action<ParentReplyModel> getReplyOnTopic;
            public event Action<ChildReplyModel> getReplyOnReply;
            public event Action<UserModel> refreshUser;



            //public event Action<TopicModel> sendMessageEvent;

            public event Action<UserModel> refreshUserEvent;
            public event Action<MessageModel> multicastMessageEvent;


            //Signals

            #endregion

            #region Connection/Disconnection

            public void Connect()
            {
                /* localhost - 127.0.0.1
                    Oleksa 172.24.226.173 
                    Ira 172.24.237.81 
                    YurAAAAAAAAAAAAAAA 172.24.101.91
                    SACHJKO 172.24.251.137  */
                _client.Connect("127.0.0.1", 7891);
                packetReader = new PacketReader(_client.GetStream());
                if (packetReader != null)
                {
                    ReadPackets();
                    SendPacket(SignalsEnum.GetAllUsers);
                    SendPacket(SignalsEnum.GetTopics);
                }
            }


            public void Disconnect()
            {
                SendPacket(SignalsEnum.Disconnect);

                _client.Close();

                _cancellationTokenSource.Cancel();
            }

            #endregion

            #region User

            public void SignUp(UserModelID user)
            {
                SendPacket(SignalsEnum.SignUp, user);
            }

            public void Login(AuthUserModel user)
            {
                SendPacket(SignalsEnum.Login, user);
            }

            public void EditUser(UserModelID user)
            {
                SendPacket(SignalsEnum.EditUser, user);
            }

            public void LogOut()
            {
                SendPacket(SignalsEnum.Logout);
            }

            public void GetBannedUsers()
            {
                SendPacket(SignalsEnum.GetBannedUsers);
            }

            public void UnbanUser(uint ID)
            {
                SendPacket(SignalsEnum.UnbanUser, ID);
            }

            #endregion

            #region SignUp
            public void GetInformationForSignUp()
            {
                SendPacket(SignalsEnum.GetStatuses);
                SendPacket(SignalsEnum.GetFieldsOfStudy);
                SendPacket(SignalsEnum.GetSpecializations);
                SendPacket(SignalsEnum.GetUniversities);
                SendPacket(SignalsEnum.GetDegrees);
            }
            #endregion

            #region Post

            public void GetTopics()
            {
                SendPacket(SignalsEnum.GetTopics);
            }

            public void CreateTopic(TopicModelID topic)
            {
                SendPacket(SignalsEnum.CreateTopic, topic);
            }

            public void EditTopic(TopicModel topic)
            {
                SendPacket(SignalsEnum.EditTopic, topic);
            }

            public void DeleteTopic(int postID)
            {
                SendPacket(SignalsEnum.DeleteTopic, new { postId = postID });
            }

            public void UpvoteTopic(int postID)
            {
                SendPacket(SignalsEnum.UpvoteTopic, new { postId = postID });
            }

            public void DownvoteTopic(int postID)
            {
                SendPacket(SignalsEnum.DownvoteTopic, new { postId = postID });
            }


            #endregion

            #region Reply

            public void CreateReply(ReplyModelID reply)
            {
                SendPacket(SignalsEnum.CreateReply, reply);
            }

            public void EditReply(ReplyModelID reply)
            {
                SendPacket(SignalsEnum.EditReply, reply);
            }

            public void DeleteReply(int replyID)
            {
                SendPacket(SignalsEnum.DeleteReply, new { replyId = replyID });
            }

            public void UpvoteReply(int replyID)
            {
                SendPacket(SignalsEnum.UpvoteReply, new { replyId = replyID });
            }

            public void DownvoteReply(int replyID)
            {
                SendPacket(SignalsEnum.DownvoteReply, new { replyId = replyID });
            }

            public void ReplyToReply(ReplyModelID reply)
            {
                SendPacket(SignalsEnum.ReplyToReply, reply);
            }
            #endregion

            #region Chat

            public void GetAllUsers()
            {
                SendPacket(SignalsEnum.GetAllUsers);
            }

            public void StartChat(ChatModelID chat)
            {
                SendPacket(SignalsEnum.StartChat, chat);
            }

            public void SendMessage(string Message)
            {
                SendPacket(SignalsEnum.SendMessage, new { message = Message });
            }

            public void DeleteMessage(int messageId)
            {
                SendPacket(SignalsEnum.DeleteMessage, new { messageID = messageId });
            }

            public void EditMessage(string Message)
            {
                SendPacket(SignalsEnum.EditMessage, new { message = Message });
            }

            public void ReadMessage(int messageId)
            {
                SendPacket(SignalsEnum.ReadMessage, new { messageID = messageId });
            }

            public void DeleteChat(int chatId)
            {
                SendPacket(SignalsEnum.DeleteChat, new { chatID = chatId });
            }



            #endregion

            #region Read Signals From Server

            private void ReadPackets()
            {
                Task.Run(() =>
                {
                    int Counter = 5;
                    while (true)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }
                        var signal = packetReader.ReadSignal();
                        switch (signal)
                        {
                            case (byte)SignalsEnum.GetAllUsers:
                                {
                                    var userModelID = packetReader.ReadPacket<List<UserModelID>>().Data;

                                    Globals.AllUsers_Cache.Clear();

                                    foreach (var user in userModelID)
                                    {
                                        UserModel temp = new UserModel(user);

                                        Globals.AllUsers_Cache.Add(temp);
                                    }

                                    packetReader.ClearStream();

                                    Debug.WriteLine($"{DateTime.Now} All users loaded. ");
                                    break;
                                }
                            //add replies when clicked on topic 
                            case (byte)SignalsEnum.GetTopics:
                                {
                                    Globals.Topics_Cache.Clear();

                                    var topicModelID = packetReader.ReadPacket<List<TopicModelID>>().Data;

                                    foreach (var topic in topicModelID)
                                    {
                                        TopicModel temp = new TopicModel(topic);

                                        temp.Author = Globals.AllUsers_Cache.Where(user => topic.AuthorID == user.ID).FirstOrDefault();

                                        Globals.Topics_Cache.Add(temp);
                                    }

                                    getTopicsEvent?.Invoke(Globals.Topics_Cache);

                                    packetReader.ClearStream();

                                    Debug.WriteLine($"{DateTime.Now} All topics loaded. ");
                                    break;
                                }

                            case (byte)SignalsEnum.GetUserChats:
                                {
                                    var userChats = packetReader.ReadPacket<List<ChatModelID>>().Data;

                                    Globals.User_Cache.Chats = [];
                                    List<uint> chatIDs = [];

                                    foreach (var chat in userChats)
                                    {
                                        ChatModel temp = new ChatModel(chat);
                                        temp.Users = chat.UserIDs
                                                        .Select(userId => Globals.AllUsers_Cache.FirstOrDefault(user => user.ID == userId))
                                                        .Where(user => user != null)
                                                        .ToList();

                                        temp.Messages = [];
                                        Globals.User_Cache.Chats.Add(temp);
                                        chatIDs.Add(chat.ID); //to get messages to all those chats by their ids
                                    }

                                    packetReader.ClearStream();
                                    Globals.server.SendPacket(SignalsEnum.GetAllUsersMessage, chatIDs);

                                    Debug.WriteLine($"Recived {userChats.Count} chats");
                                    break;
                                }

                            case (byte)SignalsEnum.GetAllUsersMessage:
                                {
                                    var allUsersMessageID = packetReader.ReadPacket<List<MessageModelID>>().Data;

                                    foreach (var messageID in allUsersMessageID)
                                    {
                                        var message = new MessageModel(messageID);

                                        message.User = Globals.AllUsers_Cache.Where(user => messageID.UserID == user.ID).FirstOrDefault();

                                        message.Chat = Globals.User_Cache.Chats.Where(chat => chat.ID == messageID.ChatID).FirstOrDefault();

                                        foreach (var chat in Globals.User_Cache.Chats)
                                        {
                                            if (messageID.ChatID == chat.ID)
                                            {
                                                chat.Messages.Add(message);
                                            }
                                        }
                                    }

                                    openChatsEvent?.Invoke(null);

                                    break;
                                }

                            case (byte)SignalsEnum.Login:
                                {
                                    var user = packetReader.ReadPacket<UserModelID>().Data;
                                    Globals.User_Cache = new UserModel(user);

                                    loginEvent?.Invoke(Globals.User_Cache);

                                    Debug.WriteLine($"User {user.Username} logged in");
                                    break;
                                }

                            case (byte)SignalsEnum.SignUp:
                                {
                                    var userModelID = packetReader.ReadPacket<UserModelID>().Data;
                                    UserModel user = new UserModel(userModelID);

                                    Globals.User_Cache = user;

                                    signUpEvent?.Invoke(Globals.User_Cache);

                                    Debug.WriteLine($"{DateTime.Now} User {user.Username} registered");
                                    break;
                                }

                            case (byte)SignalsEnum.Logout:
                                {
                                    Globals.User_Cache = null;
                                    Globals.AllUsers_Cache = [];
                                    Globals.Topics_Cache = [];

                                    SendPacket(SignalsEnum.GetAllUsers);
                                    SendPacket(SignalsEnum.GetTopics);

                                    logoutEvent?.Invoke();

                                    Debug.WriteLine($"{DateTime.Now} User loged out");
                                    break;
                                }

                            case (byte)SignalsEnum.CreateReply:
                                {
                                    var reply = packetReader.ReadPacket<ParentReplyModelID>().Data;

                                    var temp = new ParentReplyModel(reply);

                                    temp.User = Globals.AllUsers_Cache.Where(user => user.ID == reply.UserID).FirstOrDefault();
                                    temp.Topic = Globals.Topics_Cache.Where(topic => topic.ID == reply.TopicID).FirstOrDefault();

                                    Globals.Topics_Cache.Where(topic => topic.ID == reply.TopicID).FirstOrDefault().Replies.Add(temp);

                                    getReplyOnTopic.Invoke(temp);

                                    Debug.WriteLine($"{DateTime.Now} Recived reply");

                                    break;
                                }

                            case (byte)SignalsEnum.CreateReplyToReply:
                                {
                                    var reply = packetReader.ReadPacket<ChildReplyModelID>().Data;

                                    var temp = new ChildReplyModel(reply);

                                    temp.User = Globals.AllUsers_Cache.Where(user => user.ID == reply.UserID).FirstOrDefault();
                                    temp.Topic = Globals.Topics_Cache.Where(topic => topic.ID == reply.TopicID).FirstOrDefault();

                                    temp.ReplyTo = Globals.Topics_Cache.Where(topic => topic.ID == reply.TopicID)
                                    .FirstOrDefault().Replies
                                    .Where(r => r.ID == reply.RootReplyID).Select(r => r.User).FirstOrDefault();

                                    temp.RootReply = temp.Topic.Replies
                                    .Where(parentReply => parentReply.ID == reply.RootReplyID)
                                    .FirstOrDefault();

                                    Globals.Topics_Cache.Where(topic => topic.ID == reply.TopicID).FirstOrDefault().Replies
                                    .Where(parentReply => parentReply.ID == reply.RootReplyID).FirstOrDefault().Replies.Add(temp);

                                    getReplyOnReply.Invoke(temp);

                                    Debug.WriteLine($"{DateTime.Now} Recived reply to reply");

                                    break;
                                }

                            case (byte)SignalsEnum.GetReplies:
                                {
                                    var topicID = packetReader.ReadPacket<TopicModelID>().Data;

                                    Globals.OpenedTopic_Cache = new TopicModel(topicID);
                                    Globals.OpenedTopic_Cache.Author = Globals.AllUsers_Cache.Where(user => user.ID == topicID.AuthorID).FirstOrDefault();

                                    Debug.WriteLine($"{DateTime.Now} Recived topic");
                                    break;
                                }
                            case (byte)SignalsEnum.GetParentReplies:
                                {
                                    var replies = packetReader.ReadPacket<List<ParentReplyModelID>>().Data;

                                    var repliesModel = new List<ParentReplyModel>();

                                    foreach (var reply in replies)
                                    {
                                        var temp = new ParentReplyModel(reply);
                                        temp.User = Globals.AllUsers_Cache.Where(user => user.ID == reply.UserID).FirstOrDefault();
                                        temp.Topic = Globals.OpenedTopic_Cache;

                                        Globals.OpenedTopic_Cache.Replies.Add(temp);
                                    }

                                    Debug.WriteLine($"{DateTime.Now} Recived parent replies");
                                    break;
                                }
                            case (byte)SignalsEnum.GetChildReplies:
                                {
                                    var replies = packetReader.ReadPacket<List<ChildReplyModelID>>().Data;

                                    var repliesModel = new List<ChildReplyModel>();

                                    foreach (var reply in replies)
                                    {
                                        var temp = new ChildReplyModel(reply);
                                        temp.User = Globals.AllUsers_Cache.Where(user => user.ID == reply.UserID).FirstOrDefault();
                                        temp.Topic = Globals.OpenedTopic_Cache;

                                        temp.ReplyTo = temp.Topic.Replies
                                        .Where(r => r.ID == replies[0].RootReplyID).Select(r => r.User).FirstOrDefault();

                                        temp.RootReply = temp.Topic.Replies
                                        .Where(parentReply => parentReply.ID == reply.RootReplyID)
                                        .FirstOrDefault();

                                        repliesModel.Add(temp);

                                    }

                                    foreach (ParentReplyModel parentReply in Globals.OpenedTopic_Cache.Replies)
                                    {
                                        parentReply.Replies = repliesModel.Where(r => r.RootReply.ID == parentReply.ID).ToList();
                                    }

                                    openTopicEvent?.Invoke(Globals.OpenedTopic_Cache);

                                    Debug.WriteLine($"{DateTime.Now} Recived child replies");
                                    break;
                                }

                            case (byte)SignalsEnum.Refresh:
                                {
                                    var user = packetReader.ReadPacket<UserModelID>().Data;

                                    Globals.User_Cache = new UserModel(user);

                                    refreshUserEvent?.Invoke(Globals.User_Cache);

                                    Debug.WriteLine($"Refrash user {user.Username}");
                                    break;
                                }
                            case (byte)SignalsEnum.ChangeUserPhoto:
                                {
                                    var userModelID = packetReader.ReadPacket<UserModelID>().Data;

                                    Globals.User_Cache.Photo = userModelID.Photo;

                                    Globals.server.refreshUser.Invoke(Globals.User_Cache);

                                    Debug.WriteLine($"User {userModelID.Username} changed photo");

                                    break;
                                }

                            case (byte)SignalsEnum.MessageMulticast:
                                {
                                    var messageID = packetReader.ReadPacket<MessageModelID>().Data;
                                    MessageModel message = new MessageModel(messageID);
                                    message.Chat = Globals.User_Cache.Chats.Where(chat => chat.ID == messageID.ChatID).FirstOrDefault();
                                    message.User = Globals.AllUsers_Cache.Where(user => user.ID == messageID.UserID).FirstOrDefault();

                                    multicastMessageEvent?.Invoke(message);

                                    Debug.WriteLine($"Multicast message to {_client}");
                                    break;
                                }

                            case (byte)SignalsEnum.GetBannedUsers:
                                {
                                    var bannedUsersID = packetReader.ReadPacket<List<UserModelID>>().Data;
                                    List<UserModel> users = bannedUsersID.Select(user => new UserModel(user)).ToList();

                                    getBannedUsersEvent?.Invoke(users);

                                    Debug.WriteLine($"Recived {users.Count} banned users");
                                    break;
                                }

                            case (byte)SignalsEnum.GetStatuses:
                                {
                                    var statuses = packetReader.ReadPacket<List<string>>().Data;
                                    getStatusesEvent?.Invoke(statuses);
                                    Debug.WriteLine($"Recived list of statuses");
                                    break;
                                }
                            case (byte)SignalsEnum.GetFieldsOfStudy:
                                {
                                    var fields = packetReader.ReadPacket<List<string>>().Data;
                                    getFieldOfStudyEvent?.Invoke(fields);
                                    Debug.WriteLine($"Recived list of field of study");
                                    break;
                                }
                            case (byte)SignalsEnum.GetSpecializations:
                                {
                                    var specializations = packetReader.ReadPacket<List<string>>().Data;
                                    getSpecializationsEvent?.Invoke(specializations);
                                    Debug.WriteLine($"Recived slist of pesializations");
                                    break;
                                }
                            case (byte)SignalsEnum.GetUniversities:
                                {
                                    var uni = packetReader.ReadPacket<List<string>>().Data;
                                    getUniversitiesEvent?.Invoke(uni);

                                    Debug.WriteLine($"Recived list of universities");
                                    break;
                                }
                            case (byte)SignalsEnum.GetDegrees:
                                {
                                    var degrees = packetReader.ReadPacket<List<string>>().Data;
                                    getDegreesEvent?.Invoke(degrees);

                                    Debug.WriteLine($"Recived list of degrees");
                                    break;
                                }
                            case (byte)SignalsEnum.EditUser:
                                {
                                    var user = packetReader.ReadPacket<UserModelID>().Data;
                                    Globals.User_Cache = new UserModel(user);

                                    editUserEvent?.Invoke(Globals.User_Cache);
                                    break;
                                }

                            case (byte)SignalsEnum.BanUser:
                                {
                                    var user = packetReader.ReadPacket<UserModelID>().Data;

                                    Globals.AllUsers_Cache.Where(t => t.ID == user.ID).FirstOrDefault().IsBanned = true;

                                    break;
                                }
                        }



                        packetReader.Signal = 255;
                        packetReader.ClearStream();
                    }
                }, _cancellationTokenSource.Token);
            }

            #endregion

            #region Helpers

            public void SendPacket(SignalsEnum signal, uint ID)
            {
                if (_client.Connected)
                {
                    var authPacket = new PacketBuilder<uint>();
                    var packet = authPacket.GetPacketBytes(signal, ID);
                    _client.Client.Send(packet);
                }
            }

            public void SendPacket<T>(SignalsEnum signal, T user) where T : class
            {
                if (_client.Connected)
                {
                    var authPacket = new PacketBuilder<T>();
                    var packet = authPacket.GetPacketBytes(signal, user);
                    _client.Client.Send(packet);
                }
            }

            public void SendPacket(SignalsEnum signal)
            {
                if (_client.Connected)
                {
                    var authPacket = new PacketBuilder<object>();
                    var packet = authPacket.GetPacketBytes(signal);
                    _client.Client.Send(packet);
                }
            }

            #endregion
        }
    }
}

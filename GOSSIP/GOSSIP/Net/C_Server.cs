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

namespace GOSSIP.Net
{
    public static class Globals
    {
        public static Server server = new Server();

        public static UserModel User_Cache { get; set; }
        public static List<UserModel> AllUsers_Cache { get; set; } = [];
        public static List<TopicModel> Topics_Cache { get; set; } = [];

        public static void RefreshUser()
        {
            server.SendPacket(SignalsEnum.GetAllUsers);
            server.SendPacket(SignalsEnum.GetTopics);
            server.SendPacket(SignalsEnum.RefreshUser);
        }

    }

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
        public event Action editUserEvent;
        public event Action logoutEvent;
        public event Action<UserModel> loginEvent;
        public event Action<List<TopicModel>> getTopicsEvent;

        public event Action<List<string>> getStatusesEvent;
        public event Action<List<string>> getFieldOfStudyEvent;
        public event Action<List<string>> getSpecializationsEvent;
        public event Action<List<string>> getUniversitiesEvent;
        public event Action<List<string>> getDegreesEvent;
        public event Action<TopicModel> openTopicEvent;
        public event Action<List<UserModel>> getUsersEvent;
        public event Action<object> openChatsEvent;



        //public event Action<TopicModel> sendMessageEvent;

        public event Action<UserModel> refreshUserEvent;
        public event Action<MessageModel> multicastMessageEvent;


        //Signals

        #endregion

        #region Connection/Disconnection

        public void Connect()
        {
            _client.Connect("172.24.237.81", 7891);
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

        public void CreateTopic(TopicModel topic)
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

                                foreach(var topic in topicModelID)
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
                                        if(messageID.ChatID == chat.ID)
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
                        case(byte)SignalsEnum.Logout:
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
                        case (byte)SignalsEnum.GetReplies:
                            {
                                var Replies = packetReader.ReadPacket<List<ReplyModelID>>().Data;
                                TopicModel topic = new TopicModel();

                                //Code to connect replies to topic

                                openTopicEvent?.Invoke(topic);

                                Debug.WriteLine($"{DateTime.Now} Recived replies");
                                break;
                            }
                        case (byte)SignalsEnum.RefreshUser:
                            {
                                var user = packetReader.ReadPacket<UserModelID>().Data;

                                Globals.User_Cache = new UserModel(user);

                                refreshUserEvent?.Invoke(Globals.User_Cache);

                                Debug.WriteLine($"Refrash user {user.Username}");
                                break;
                            }
                        case (byte)SignalsEnum.MessageMulticast:
                            {
/*                                var message = packetReader.ReadPacket<MessageModelID>().Data;
                                multicastMessageEvent?.Invoke(message);
                                Debug.WriteLine($"Multicast message to {_client}");*/
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using GOSSIP.Net.IO;
using GOSSIP.Models;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GOSSIP.Net
{
    public static class Globals
    {
        public static Server server = new Server();
    }

    public class Server
    {
        TcpClient _client;

        public PacketReader packetReader; 

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Server()
        {
            _client = new TcpClient();
           // _networkStream = new NetworkStream(_client.Client);
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

        public event Action<List<UserModel>> getUsersEvent;



        //public event Action<TopicModel> sendMessageEvent;

        public event Action<UserModel> refreshUserEvent;
        public event Action<MessageModel> multicastMessageEvent;


        //Signals

        #endregion

        #region Connection/Disconnection

        public void Connect()
        {
            _client.Connect("172.22.237.81", 7891);
            packetReader = new PacketReader(_client.GetStream());
            if (packetReader != null)
            {
                SendPacket(SignalsEnum.GetTopics);
                ReadPackets();
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

        public void SignUp(UserModel user)
        {
            SendPacket(SignalsEnum.SignUp, user);
        }

        public void Login(AuthUserModel user)
        {
            SendPacket(SignalsEnum.Login, user);
        }

        public void EditUser(UserModel user)
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

        public void CreateReply(ReplyModel reply)
            {
                SendPacket(SignalsEnum.CreateReply, reply);
            }

            public void EditReply(ReplyModel reply)
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

            public void ReplyToReply(ReplyModel reply)
            {
                SendPacket(SignalsEnum.ReplyToReply, reply);
            }
            #endregion

        #region Chat

            public void GetAllUsers()
            {
                 SendPacket(SignalsEnum.GetAllUsers);
            }

        public void StartChat(ChatModel chat)
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
                        case (byte)SignalsEnum.GetTopics:
                            {
                                try
                                {
                                    var rawPacket = packetReader.ReadRawPacket();
                                    var packet = packetReader.DeserializePacket<Packet<List<TopicModel>>>(rawPacket);
                                    List<TopicModel> topics = packet.Data;
                                    getTopicsEvent?.Invoke(topics);
                                    packetReader.ClearStream();
                                    Counter = 5;
                                }
                                catch (Exception)
                                {
                                    if (Counter != 0)
                                    {
                                        packetReader.ClearStream();
                                        SendPacket(SignalsEnum.GetTopics);
                                        Counter--;
                                        Debug.WriteLine($"{DateTime.Now} Retrying to get topics... Counter {Counter}");
                                    }
                                }
                                break;
                            }
                        case (byte)SignalsEnum.Login:
                            {
                                var user = packetReader.ReadPacket<UserModel>().Data;
                                loginEvent?.Invoke(user);

                                Debug.WriteLine($"User {user.Username} logged in");
                                break;
                            }
                        case (byte)SignalsEnum.SignUp:
                            {
                                var userModel = packetReader.ReadPacket<UserModel>().Data;
                                signUpEvent?.Invoke(userModel);

                                Debug.WriteLine($"{DateTime.Now} User {userModel.Username} registered");
                                break;
                            }
                        case (byte)SignalsEnum.RefreshUser:
                            {
                                var user = packetReader.ReadPacket<UserModel>().Data;
                                refreshUserEvent?.Invoke(user);
                                Debug.WriteLine($"Refrash user {_client}");
                                break;
                            }

                        case (byte)SignalsEnum.MessageMulticast:
                            {
                                var message = packetReader.ReadPacket<MessageModel>().Data;
                                multicastMessageEvent?.Invoke(message);
                                Debug.WriteLine($"Multicast message to {_client}");

                                break;
                            }
                        case (byte)SignalsEnum.GetStatuses:
                            {
                                var statuses = packetReader.ReadPacket<List<string>>().Data;
                                getStatusesEvent?.Invoke(statuses);
                                Debug.WriteLine(statuses);
                                break;
                            }
                        case (byte)SignalsEnum.GetFieldsOfStudy:
                            {
                                var fields = packetReader.ReadPacket<List<string>>().Data;
                                getFieldOfStudyEvent?.Invoke(fields);
                                break;
                            }
                        case (byte)SignalsEnum.GetSpecializations:
                            {
                                var specializations = packetReader.ReadPacket<List<string>>().Data;
                                getSpecializationsEvent?.Invoke(specializations);
                                break;
                            }
                        case (byte)SignalsEnum.GetUniversities:
                            {
                                var uni = packetReader.ReadPacket<List<string>>().Data;
                                getUniversitiesEvent?.Invoke(uni);
                                break;
                            }
                        case (byte)SignalsEnum.GetDegrees:
                            {
                                var degrees = packetReader.ReadPacket<List<string>>().Data;
                                getDegreesEvent?.Invoke(degrees);
                                break;
                            }
                        case (byte)SignalsEnum.GetAllUsers:
                            {
                                var users = packetReader.ReadPacket<List<UserModel>>().Data;
                                getUsersEvent?.Invoke(users);
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

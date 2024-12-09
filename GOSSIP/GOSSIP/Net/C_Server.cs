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

        public Dictionary<Guid, byte[]> unacknowledgedPackets = new Dictionary<Guid, byte[]>();
        public Mutex mutex = new Mutex();

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



        //public event Action<TopicModel> sendMessageEvent;

        public event Action<UserModel> refreshUserEvent;

        //Signals

        #endregion

        #region Connection/Disconnection

        public void Connect()
        {
            _client.Connect("172.22.237.81", 7891);
            packetReader = new PacketReader(_client.GetStream());
            if (packetReader != null)
            {
                SendPacket<object>(SignalsEnum.GetTopics);
                ReadPackets();
            }
        }

       
        public void Disconnect()
        {
            SendPacket<object>(SignalsEnum.Disconnect);

            _client.Close();

            _cancellationTokenSource.Cancel();
        }

        #endregion

        #region Senders



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
            SendPacket<object>(SignalsEnum.Logout);
            
        }

        #endregion

        #region Post

        public void GetTopics()
        {
            SendPacket<object>(SignalsEnum.GetTopics);
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

        #region Getters

        #endregion

        #region Read Signals From Server

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                Guid packetId = Guid.Empty;

                try
                {

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
                                    var rawPacket = packetReader.ReadRawPacket();
                                    var packet = packetReader.DeserializePacket<Packet<List<TopicModel>>>(rawPacket);
                                    List<TopicModel> topics = packet.Data;
                                    packetId = packet.PacketId;
                                    getTopicsEvent?.Invoke(topics);
                                    packetReader.ClearStream();

                                    break;
                                }
                            case (byte)SignalsEnum.Login:
                                {
                                    var packet = packetReader.ReadPacket<Packet<UserModel>>().Data;
                                    packetId = packet.PacketId;
                                    loginEvent?.Invoke(packet.Data);

                                    Debug.WriteLine($"User {packet.Data.Username} logged in");
                                    break;
                                }
                            case (byte)SignalsEnum.SignUp:
                                {
                                    var packet = packetReader.ReadPacket<Packet<UserModel>>().Data;
                                    signUpEvent?.Invoke(packet.Data);

                                    Debug.WriteLine($"{DateTime.Now} User {packet.Data.Username} registered");
                                    break;
                                }
                            case (byte)SignalsEnum.RefreshUser:
                                {
                                    var packet = packetReader.ReadPacket<Packet<UserModel>>().Data;
                                    refreshUserEvent?.Invoke(packet.Data);
                                    Debug.WriteLine($"Refrash user {_client}");
                                    break;
                                }

                        }
                        packetReader.Signal = 255;
                        packetReader.ClearStream();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    SendAcknowledgement(packetId);
                }

            }, _cancellationTokenSource.Token);
        }

        #endregion

        #region Helpers

        public void SendPacket<T>(SignalsEnum signal, T data = null) where T : class
        {
            var packetBuilder = new PacketBuilder<T>();
            var packetBytes = packetBuilder.GetPacketBytes(signal, data);
            var packetId = packetBuilder.PacketId;

            mutex.WaitOne();
            unacknowledgedPackets[packetId] = packetBytes;
            mutex.ReleaseMutex();

            _client.Client.Send(packetBytes);

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
                        _client.Client.Send(packetBytes);

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
            _client.Client.Send(ackPacketBytes);
        }

        #endregion
    }
}

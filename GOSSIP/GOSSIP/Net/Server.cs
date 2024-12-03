using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using GOSSIP.Net.IO;
using GOSSIP.Models;

namespace GOSSIP.Net
{
    public class Server
    {
        TcpClient _client;

        public PacketReader packetReader;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        //evens gets from server

        #region Events
        public event Action connectedEvent;
        public event Action userDisonnectedEvent;

        public event Action registerEvent;
        public event Action editUserEvent;
        public event Action loginEvent;
        public event Action logoutEvent;

        //Signals

        #endregion

        #region Connection/Disconnection

        public void Connect()
        {
            _client.Connect("172.22.237.81", 7891);
            packetReader = new PacketReader(_client.GetStream());
            if (packetReader != null)
            {
                ConnectedToServer();
                ReadPackets();
            }
        }

        public void ConnectedToServer()
        {
            SendPacket(SignalsEnum.Connect, new UserModel { Username = "Guest" });
        }

        public void Disconnect()
        {
            _client.Close();

            _cancellationTokenSource.Cancel();

            // all VMs = null...
        }

        #endregion

        #region Senders

        

        public void Register(UserModel user)
        {
            SendPacket(SignalsEnum.Register, user);
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
            Disconnect();
            Connect();
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

            public void ReplyToTopic(ReplyModel reply)
            {
                SendPacket(SignalsEnum.ReplyToTopic, reply);
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
                    while (true)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }
                        var signal = packetReader.Signal;
                        switch (signal)
                        {
                            case 0:
                                connectedEvent?.Invoke();
                                break;

                            case 255:
                                userDisonnectedEvent?.Invoke();
                                break;
                            default:
                                Console.WriteLine("Unknown signal");
                                break;
                        }
                    }
                }, _cancellationTokenSource.Token);
            }

            #endregion

            #region Helpers

            private void SendPacket<T>(SignalsEnum signal, T user) where T : class
            {
                if (_client.Connected)
                {
                    var authPacket = new PacketBuilder<T>();
                    var packet = authPacket.GetPacketBytes(signal, user);
                    _client.Client.Send(packet);
                }
            }

            private void SendPacket(SignalsEnum signal)
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

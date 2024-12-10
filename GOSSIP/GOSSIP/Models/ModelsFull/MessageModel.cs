using System.Text.Json.Serialization;

namespace GOSSIP
{
    [Serializable]
    public class MessageModel
    {
        public uint ID { get; set; }
        public ChatModel Chat { get; set; }
        public UserModel User { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }

        public MessageModel(uint id, ChatModel chat, UserModel user,
            string messageText, DateTime timeStamp, bool isRead, bool isDeleted)
        {
            ID = id;
            Chat = chat;
            User = user;
            MessageText = messageText;
            TimeStamp = timeStamp;
            IsRead = isRead;
            IsDeleted = isDeleted;
        }

        public MessageModel() { }

        public MessageModel(MessageModelID messageModelID)
        {
            ID = messageModelID.ID;
            Chat = null;
            User = null;
            MessageText = messageModelID.MessageText;
            TimeStamp = messageModelID.TimeStamp;
            IsRead = messageModelID.IsRead;
            IsDeleted = messageModelID.IsDeleted;
        }
    }
}
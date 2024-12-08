using System.Text.Json.Serialization;

namespace GOSSIP.Models
{
    //Тимчасова модель повідомлень. Пізніше буде замінена на робочу 
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
            chat = Chat;
            User = user;
            MessageText = messageText;
            TimeStamp = timeStamp;
            IsRead = isRead;
            IsDeleted = isDeleted;
        }

        public MessageModel()
        {
            
        }
    }
}


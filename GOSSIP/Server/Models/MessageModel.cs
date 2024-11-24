using System.Text.Json.Serialization;

namespace Server.Models
{
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

        public string FormattedTime => TimeStamp.ToString("hh:mm");

        // Перевизначення ToString() TODO: чи потрібно?
        //public override string ToString()
        //{
        //    return $"{(IsSentByCurrentUser ? "You" : $"Sender {SenderID}")}: {MessageText} at {FormattedTime}";
        //}
    }
}


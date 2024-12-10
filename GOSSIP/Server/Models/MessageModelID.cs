using System.Text.Json.Serialization;

namespace Server
{
    [Serializable]
    public class MessageModelID
    {
        public uint ID { get; set; }
        public uint ChatID { get; set; }
        public uint UserID { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }

        public MessageModelID(uint id, uint chatID, uint userID,
            string messageText, DateTime timeStamp, bool isRead, bool isDeleted)
        {
            ID = id;
            ChatID = chatID;
            UserID = userID;
            MessageText = messageText;
            TimeStamp = timeStamp;
            IsRead = isRead;
            IsDeleted = isDeleted;
        }

        public MessageModelID()
        {

        }
    }
}
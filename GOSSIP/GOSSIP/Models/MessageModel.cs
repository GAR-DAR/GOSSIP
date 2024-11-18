namespace GOSSIP.Models
{
    //Тимчасова модель повідомлень. Пізніше буде замінена на робочу 
    public class MessageModel(int id, int chatID, int senderID, bool isSentByCurrentUser,
        string messageText, DateTime timeStamp, bool isRead, bool isDeleted)
    {
        public int ID { get; set; } = id;
        public int ChatID { get; set; } = chatID;
        public int SenderID { get; set; } = senderID;
        public bool IsSentByCurrentUser { get; set; } = isSentByCurrentUser;
        public string MessageText { get; set; } = messageText;
        public string TimeStamp { get; set; } = timeStamp.ToString(@"hh\:mm");
        public bool IsRead { get; set; } = isRead;
        public bool IsDeleted { get; set; } = isDeleted;

        override public string ToString()
        {
            return MessageText;
        }
    }
}

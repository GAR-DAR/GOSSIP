using System.Text.Json.Serialization;

namespace GOSSIP.Models
{
    //Тимчасова модель повідомлень. Пізніше буде замінена на робочу 
    [Serializable]
    public class MessageModel
    {
        public int ID { get; set; }
        public int ChatID { get; set; }
        public int SenderID { get; set; }
        public bool IsSentByCurrentUser { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; } // Зберігаємо як DateTime
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }

        // Конструктор
        public MessageModel(int id, int chatID, int senderID, bool isSentByCurrentUser,
                            string messageText, DateTime timeStamp, bool isRead, bool isDeleted)
        {
            ID = id;
            ChatID = chatID;
            SenderID = senderID;
            IsSentByCurrentUser = isSentByCurrentUser;
            MessageText = messageText;
            TimeStamp = timeStamp;
            IsRead = isRead;
            IsDeleted = isDeleted;
        }

        // Форматована строка часу
        public string FormattedTime => TimeStamp.ToString("hh:mm");

        // Перевизначення ToString()
        public override string ToString()
        {
            return $"{(IsSentByCurrentUser ? "You" : $"Sender {SenderID}")}: {MessageText} at {FormattedTime}";
        }
    }
}


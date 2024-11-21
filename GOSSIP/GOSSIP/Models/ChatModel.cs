using System.Collections.ObjectModel;

namespace GOSSIP.Models
{
    //Тимчасова модель чатів. Пізніше буде замінена на робочу модель
    public class ChatModel
    {
        public User Interlocutor { get; set; }
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }

        public ChatModel(int id, DateTime createdAt, bool isDeleted, User user, ObservableCollection<MessageModel> messages)
        {
            ID = id;
            CreatedAt = createdAt;
            IsDeleted = isDeleted;
            Interlocutor = user;
            Messages = messages;
        }

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }

}

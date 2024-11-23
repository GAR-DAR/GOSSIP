using System.Collections.ObjectModel;

namespace GOSSIP.Models
{
    //Тимчасова модель чатів. Пізніше буде замінена на робочу модель
    [Serializable]
    public class ChatModel
    {
        public int ID { get; set; }
        public List<UserModel> Users { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }

        // Конструктор
        public ChatModel(List<UserModel> users, DateTime createdAt, bool isDeleted, 
            ObservableCollection<MessageModel> messages, int iD)
        {
            this.Users = users;
            this.CreatedAt = createdAt;
            this.IsDeleted = isDeleted;
            this.Messages = messages;
            this.ID = iD;
        }

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }

}

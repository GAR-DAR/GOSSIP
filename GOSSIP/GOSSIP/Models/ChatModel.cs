using System.Collections.ObjectModel;

namespace GOSSIP.Models
{
    //Тимчасова модель чатів. Пізніше буде замінена на робочу модель
    [Serializable]
    public class ChatModel
    {
        public UserModel Interlocutor { get; set; }
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }

        // Конструктор
        public ChatModel(UserModel Interlocutor, int ID, DateTime CreatedAt, bool IsDeleted, ObservableCollection<MessageModel> Messages)
        {
            this.Interlocutor = Interlocutor;
            this.ID = ID;
            this.CreatedAt = CreatedAt;
            this.IsDeleted = IsDeleted;
            this.Messages = Messages;
        }

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }

}

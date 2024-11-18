using System.Collections.ObjectModel;

namespace GOSSIP.Models
{
    //Temporary chat model. Properties will be changed according to the DB.
    public class ChatModel(int id, DateTime createdAt, bool isDeleted, User user, ObservableCollection<MessageModel> messages)
    {
        public User Interlocutor { get; set; } = user;
        public int ID { get; set; } = id;
        public DateTime CreatedAt { get; set; } = createdAt;
        public bool IsDeleted { get; set; } = isDeleted;
        public ObservableCollection<MessageModel> Messages { get; set; } = messages;

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }
}

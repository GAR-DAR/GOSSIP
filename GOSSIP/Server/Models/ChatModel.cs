using System.Collections.ObjectModel;

namespace Server.Models
{
    public class ChatModel
    {
        public uint ID { get; set; }
        public List<UserModel> Users { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<MessageModel> Messages { get; set; }

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }

}

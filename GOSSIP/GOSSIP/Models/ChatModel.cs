using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    //Temporary chat model. Properties will be changed according to the DB.
    public class ChatModel(int id, DateTime createdAt, bool isDeleted, string name, string iconName, List<MessageModel> messages)
    {
        public int ID { get; set; } = id;
        public DateTime CreatedAt { get; set; } = createdAt;
        public bool IsDeleted { get; set; } = isDeleted;
        public string IconName = iconName;
        public string Name { get; set; } = name;
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";
        public List<MessageModel> Messages { get; set; } = messages;

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }
}

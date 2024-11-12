using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class ChatModel(string chatName, string lastMessage, string iconName, ObservableCollection<MessageModel> messages)
    {
        public string IconName = iconName;

        public string ChatName { get; set; } = chatName;
        public string LastMessage { get; set; } = lastMessage;
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";
        public ObservableCollection<MessageModel> Messages { get; set; } = messages;
    }
}

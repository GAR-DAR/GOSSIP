using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class OpenedChatVM(ChatModel chat) : ObservableObject
    {
        public string ChatName { get; set; } = chat.ChatName;
        public string LastMessage { get; set; } = chat.LastMessage;
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{chat.IconName}";
        public ObservableCollection<MessageModel> Messages { get; set; } = chat.Messages;
    }
}

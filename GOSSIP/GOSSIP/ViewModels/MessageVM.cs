using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class MessageVM(MessageModel message) : ObservableCollection<MessageModel>
    {
        public string Sender { get; set; } = message.Sender;
        public bool IsSentByCurrentUser { get; set; } = message.IsSentByCurrentUser;
        public string MessageText { get; set; } = message.MessageText;
        public string TimeStamp { get; set; } = message.TimeStamp;
    }
}

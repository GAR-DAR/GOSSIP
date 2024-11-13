using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class MessagesListVM(List<MessageModel> messages) : ObservableCollection<MessageModel>
    {
        public ObservableCollection<MessageModel> Messages { get; set; } = new(messages);

        public void AddMessage(MessageModel message)
        {
            Messages.Add(message);
        }
    }
}

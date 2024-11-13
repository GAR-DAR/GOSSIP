using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class MessagesListModel : ObservableCollection<MessageModel>
    {
        private List<MessageModel> _messages;

        public List<MessageModel> GetMessages()
        {
            return _messages;
        }

        public void AddMessage(MessageModel message)
        {
            _messages.Add(message);
        }
    }
}

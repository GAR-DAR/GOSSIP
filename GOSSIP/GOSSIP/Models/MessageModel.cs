using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class MessageModel (string sender, bool isSentByCurrentUser, string messageText, TimeSpan timeStamp)
    {
        public string Sender { get; set; } = sender;
        public bool IsSentByCurrentUser { get; set; } = isSentByCurrentUser;
        public string MessageText { get; set; } = messageText;
        public string TimeStamp { get; set; } = timeStamp.ToString(@"hh\:mm");
    }
}

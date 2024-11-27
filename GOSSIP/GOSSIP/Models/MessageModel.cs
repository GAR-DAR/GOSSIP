using System.Text.Json.Serialization;

namespace GOSSIP.Models
{
    public class MessageModel
    {
        public uint ID { get; set; }
        public ChatModel Chat { get; set; }
        public UserModel User { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
    }
}


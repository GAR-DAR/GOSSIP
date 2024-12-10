using System.Collections.ObjectModel;

namespace Server.Models
{
    public class ChatModelID
    {
        public uint ID { get; set; }
        public List<uint> UserIDs { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<uint> MessageIDs { get; set; }
    }

}

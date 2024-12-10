using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Server
{
    //Тимчасова модель чатів. Пізніше буде замінена на робочу модель
    [Serializable]
    public class ChatModelID
    {
        public uint ID { get; set; }
        public List<uint> UserIDs { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<uint> MessageIDs { get; set; }

        public void AddMessage(uint message)
        {
            MessageIDs.Add(message);
        }

        public ChatModelID(uint iD, List<uint> userIDs, string name, 
            DateTime createdAt, bool isDeleted, List<uint> messageIDs)
        {
            ID = iD;
            UserIDs = userIDs;
            Name = name;
            CreatedAt = createdAt;
            IsDeleted = isDeleted;
            MessageIDs = messageIDs;
        }

        public ChatModelID() { }
    }

}
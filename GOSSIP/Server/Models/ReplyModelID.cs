using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public abstract class ReplyModelID
    {
        public uint ID { get; set; }
        public uint UserID { get; set; }
        public uint TopicID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsDeleted { get; set; }
    }
}

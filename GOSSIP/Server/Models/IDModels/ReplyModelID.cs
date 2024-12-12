using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server
{
    [JsonDerivedType(typeof(ReplyModelID), typeDiscriminator: "ReplyModelID")]
    [JsonDerivedType(typeof(ParentReplyModelID), typeDiscriminator: "ParentReplyModelID")]
    [JsonDerivedType(typeof(ChildReplyModelID), typeDiscriminator: "ChildReplyModelID")]

    public class ReplyModelID
    {
        public uint ID { get; set; }
        public uint UserID { get; set; }
        public uint TopicID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsDeleted { get; set; }

        public ReplyModelID(uint iD, uint userID, uint topicID,
            string content, DateTime createdAt, int rating, bool isDeleted)
        {
            ID = iD;
            UserID = userID;
            TopicID = topicID;
            Content = content;
            CreatedAt = createdAt;
            Rating = rating;
            IsDeleted = isDeleted;
        }

        public ReplyModelID() { }
    }
}

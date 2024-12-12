using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GOSSIP
{
    [JsonDerivedType(typeof(ReplyModelID), typeDiscriminator: "ReplyModelID")]
    [JsonDerivedType(typeof(ChildReplyModelID), typeDiscriminator: "ChildReplyModelID")]
    [JsonDerivedType(typeof(ParentReplyModelID), typeDiscriminator: "ParentReplyModelID")]
    public class ReplyModelID
    {
       
        public uint ID { get; set; }
        public uint UserID { get; set; }
        public uint TopicID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsDeleted { get; set; }

        public ReplyModelID() { }

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

        public ReplyModelID(ReplyModel replyModel)
        {
            ID = replyModel.ID;
            UserID = replyModel.User.ID;
            TopicID = replyModel.Topic.ID;
            Content = replyModel.Content;
            CreatedAt = replyModel.CreatedAt;
            Rating = replyModel.Rating;
            IsDeleted = replyModel.IsDeleted;
        }

    }
}

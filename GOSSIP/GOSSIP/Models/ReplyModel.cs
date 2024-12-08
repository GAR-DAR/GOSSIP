using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class ReplyModel
    {
        public uint ID { get; set; }
        public UserModel User { get; set; }
        public TopicModel Topic { get; set; }
        public ReplyModel ParentReply { get; set; } = null;
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsDeleted { get; set; }

        public ReplyModel(uint iD, UserModel user, TopicModel topic, ReplyModel parentReply,
            string content, DateTime createdAt, int rating, bool isDeleted)
        {
            ID = iD;
            User = user;
            Topic = topic;
            ParentReply = parentReply;
            Content = content;
            CreatedAt = createdAt;
            Rating = rating;
            IsDeleted = isDeleted;
        }

        public ReplyModel() { }
    }
}
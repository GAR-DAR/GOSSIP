using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class ReplyModel(int id, UserModel user, TopicModel topic, ReplyModel parentReply, string content, DateTime createdAt, int rating, bool isDeleted)
    {
        public int ID { get; set; } = id;
        public UserModel User { get; set; } = user;
        public TopicModel Topic { get; set; } = topic;
        public ReplyModel ParentReply { get; set; } = parentReply;
        public string Content { get; set; } = content;
        public DateTime CreatedAt { get; set; } = createdAt;
        public int Rating { get; set; } = rating;
        public bool isDeleted { get; set; } = isDeleted;
    }
}
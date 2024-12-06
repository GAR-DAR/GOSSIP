using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ChildReplyModel : ReplyModel
    {
        public UserModel ReplyTo { get; set; }

        public ChildReplyModel(uint iD, UserModel user, TopicModel topic,
            string content, DateTime createdAt, int rating, bool isDeleted, UserModel replyTo) 
            : base(iD, user, topic, content, createdAt, rating, isDeleted)
        {
            ReplyTo = replyTo;
            Content = "@" + ReplyTo.Username + ", " + content;
        }

        public ChildReplyModel() { }
    }
}

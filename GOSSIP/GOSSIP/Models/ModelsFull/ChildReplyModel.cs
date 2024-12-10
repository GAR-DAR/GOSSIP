using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP
{
    public class ChildReplyModel : ReplyModel
    {
        public ParentReplyModel RootReply { get; set; }
        public UserModel ReplyTo { get; set; }

        public ChildReplyModel() { }

        public ChildReplyModel(uint iD, UserModel user, TopicModel topic,
            string content, DateTime createdAt, int rating, bool isDeleted,
            ParentReplyModel rootReply, UserModel replyTo)
        {
            ID = iD;
            User = user;
            Topic = topic;
            Content = content;
            CreatedAt = createdAt;
            Rating = rating;
            IsDeleted = isDeleted;
            RootReply = rootReply;
            ReplyTo = replyTo;
        }


    }

   
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ParentReplyModel : ReplyModel
    {
        public List<ChildReplyModel> Replies { get; set; } = [];

        public ParentReplyModel(uint iD, UserModel user, TopicModel topic,
            string content, DateTime createdAt, int rating, bool isDeleted, List<ChildReplyModel> replies) 
            : base(iD, user, topic, content, createdAt, rating, isDeleted)
        {
            Replies = replies;
        }

        public ParentReplyModel() { }
    } 
}

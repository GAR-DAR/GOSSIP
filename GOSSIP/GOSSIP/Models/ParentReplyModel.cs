using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class ParentReplyModel : ReplyModel1
    {
        public List<ChildReplyModel> Replies { get; set; } = [];

        public ParentReplyModel(uint iD, UserModel user, TopicModel topic,
            string content, DateTime createdAt, int rating, bool isDeleted, List<ChildReplyModel> replies) 
            : base(iD, user, topic, content, createdAt, rating, isDeleted)
        {
            Replies = replies;
        }
    } 
}

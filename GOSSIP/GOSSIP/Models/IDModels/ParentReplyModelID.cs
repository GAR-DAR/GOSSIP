using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP
{
    public class ParentReplyModelID : ReplyModelID
    {
        public List<uint> Replies { get; set; } = [];

        public ParentReplyModelID() { }

        public ParentReplyModelID(uint iD, uint userID, uint topicID,
        string content, DateTime createdAt, int rating, bool isDeleted, List<uint> replies)
        : base(iD, userID, topicID, content, createdAt, rating, isDeleted)
        {
            Replies = replies;
        }

        public ParentReplyModelID(ParentReplyModel replyModel)
        {
            foreach(var reply in replyModel.Replies)
            {
                Replies.Add(reply.ID);
            }
        }
    }
}

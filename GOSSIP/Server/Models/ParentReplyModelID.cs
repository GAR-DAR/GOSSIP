using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ParentReplyModelID : ReplyModel
    {
        public List<uint> Replies { get; set; } = [];

        public ParentReplyModelID(uint iD, uint userID,
            string content, DateTime createdAt, int rating, bool isDeleted, List<uint> replies) 
            : base(iD, userID, content, createdAt, rating, isDeleted)
        {
            Replies = replies;
        }

        public ParentReplyModelID() { }
    } 
}

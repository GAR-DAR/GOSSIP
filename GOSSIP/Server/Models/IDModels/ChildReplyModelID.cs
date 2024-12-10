using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ChildReplyModelID : ReplyModel
    {
        public uint RootReplyID { get; set; }
        public uint ReplyToUserID { get; set; }

        public ChildReplyModelID(uint iD, uint userID, uint topicID,
            string content, DateTime createdAt, int rating, bool isDeleted, uint replyToUserID) 
            : base(iD, userID, topicID, content, createdAt, rating, isDeleted)
        {
            ReplyToUserID = replyToUser;
            Content = "@" + ReplyToUserID.Username + ", " + content;
        }

        public ChildReplyModelID() { }
    }
}

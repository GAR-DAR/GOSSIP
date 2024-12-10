using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP
{

    public class ChildReplyModelID : ReplyModelID
    {
        public uint RootReplyID { get; set; }
        public uint ReplyToUserID { get; set; }

        public ChildReplyModelID() { }

        public ChildReplyModelID(uint iD, uint userID, uint topicID,
            string content, DateTime createdAt, int rating, bool isDeleted, uint replyToUserID)
            : base(iD, userID, topicID, content, createdAt, rating, isDeleted)
        {
            ReplyToUserID = replyToUserID;
            //Content = "@" + ReplyToUserID.Username + ", " + content;
        }

        public ChildReplyModelID(ChildReplyModel childReplyModel)
        {
            RootReplyID = childReplyModel.RootReply.ID;
            ReplyToUserID = childReplyModel.ReplyTo.ID;
        }
    }
}

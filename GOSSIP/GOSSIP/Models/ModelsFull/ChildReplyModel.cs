
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
    }
}
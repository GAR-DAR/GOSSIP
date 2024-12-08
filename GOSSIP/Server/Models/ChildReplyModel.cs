﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ChildReplyModel : ReplyModel
    {
        public ReplyModel RootReply { get; set; }
        public UserModel ReplyTo { get; set; }

        public ChildReplyModel() { }
    }
}
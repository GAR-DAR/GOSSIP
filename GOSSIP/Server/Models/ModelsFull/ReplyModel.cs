﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class ReplyModel
    {
        public uint ID { get; set; }
        public UserModel User { get; set; }
        public TopicModel Topic { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsDeleted { get; set; }
    }
}
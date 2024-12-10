﻿using System.Text.Json.Serialization;

namespace Server.Models
{
    public class MessageModelID
    {
        public uint ID { get; set; }
        public uint ChatID { get; set; }
        public uint UserID { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
    }
}


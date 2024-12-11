﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class UserModelID
    {
        public uint ID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? Specialization { get; set; }
        public string? University { get; set; }
        public uint? Term { get; set; }
        public string? Degree { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public string? Photo { get; set; }
        public List<uint> ChatsID { get; set; } = [];
        public Dictionary<uint, int> TopicVotes { get; set; } = [];
        public Dictionary<uint, int> ReplyVotes { get; set; } = [];
    }
}
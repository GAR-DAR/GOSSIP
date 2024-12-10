using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GOSSIP
{
    [Serializable]
    public class UserModelID
    {
        public uint ID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string FieldOfStudy { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public uint? Term { get; set; }
        public string Degree { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public string Photo { get; set; }

        public List<uint> ChatsID { get; set; } = [];

        public Dictionary<uint, int> TopicVotes { get; set; } = [];
        public Dictionary<uint, int> ReplyVotes { get; set; } = [];

        public UserModelID() { }

        public UserModelID(uint id, string email, string username, string password,
            string status, string fieldOfStudy, string specialization,
            string university, uint? term, string degree, string role,
            DateTime createdAt, bool isBanned, string photo, List<uint> chatsID)
        {
            ID = id;
            Email = email;
            Username = username;
            Password = password;
            Status = status;
            FieldOfStudy = fieldOfStudy;
            Specialization = specialization;
            University = university;
            Term = term;
            Degree = degree;
            Role = role;
            CreatedAt = createdAt;
            IsBanned = isBanned;
            Photo = photo;
            ChatsID = chatsID;
        }

        public UserModelID(UserModel userModel)
        {
            ID = userModel.ID;
            Email = userModel.Email;
            Username = userModel.Username;
            Password = userModel.Password;
            Status = userModel.Status;
            FieldOfStudy = userModel.FieldOfStudy;
            Specialization = userModel.Specialization;
            University = userModel.University;
            Term = userModel.Term;
            Degree = userModel.Degree;
            Role = userModel.Role;
            CreatedAt = userModel.CreatedAt;
            IsBanned = userModel.IsBanned;
            Photo = userModel.Photo;
            foreach(var chat in userModel.Chats)
            {
                ChatsID.Add(chat.ID);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GOSSIP
{

    [Serializable]
    public class UserModel
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

        public List<ChatModel> Chats { get; set; } = [];


        public Dictionary<uint, int> TopicVotes { get; set; } = [];
        public Dictionary<uint, int> ReplyVotes { get; set; } = [];

        public UserModel() { }

        public UserModel(uint id, string email, string username, string password,
            string status, string fieldOfStudy, string specialization,
            string university, uint term, string degree, string role,
            DateTime createdAt, bool isBanned, string photo, List<ChatModel> chats)
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
            Chats = chats;
        }

        public UserModel(UserModelID user)
        {
            ID = user.ID;
            Email = user.Email;
            Username = user.Username;
            Password = user.Password;
            Status = user.Status;
            FieldOfStudy = user.FieldOfStudy;
            Specialization = user.Specialization;
            University = user.University;
            Term = user.Term;
            Degree = user.Degree;
            Role = user.Role;
            CreatedAt = user.CreatedAt;
            IsBanned = user.IsBanned;
            Photo = user.Photo;
            Chats = null;
        }
    }
}
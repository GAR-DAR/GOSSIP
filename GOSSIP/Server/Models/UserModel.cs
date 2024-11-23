﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    //Повна модель користувача з потрібними для реєстрації полями. Буде змінена, of course

    [Serializable]
    public class UserModel
    {
        public uint ID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Status { get; set; }
        public string FieldOfStudy { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public int Term { get; set; }
        public string Degree { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        // TODO
        /*public string IconName { get; set; }
        //Поки іконки беруться із папки /Images/TempUserIcons.
        public string IconPath
        {
            get;
            set
            {
                for client
            }
        }*/

        public List<ChatModel> Chats { get; set; }

        public UserModel(uint iD, string email, string username, string status,
            string fieldOfStudy, string specialization, string university, int term, List<ChatModel> chats, string degree, 
            string role, DateTime createdAt, bool isBanned)
        {
            ID = iD;
            Email = email;
            Username = username;
            Status = status;
            FieldOfStudy = fieldOfStudy;
            Specialization = specialization;
            University = university;
            Term = term;
            //IconName = iconName;
            Chats = chats;
            Degree = degree;
            Role = role;
            CreatedAt = createdAt;
            IsBanned = isBanned;
        }
    }
}

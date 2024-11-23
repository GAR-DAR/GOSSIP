using System;
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
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string FieldOfStudy { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public int Term { get; set; }
        public string IconName { get; set; }
        //Поки іконки беруться із папки /Images/TempUserIcons.
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{IconName}";
        public List<ChatModel> Chats { get; set; }

        public UserModel(string email, string username, string password, string status,
            string fieldOfStudy, string specialization, string university, int term, string iconName, List<ChatModel> chats)
        {
            Email = email;
            Username = username;
            Password = password;
            Status = status;
            FieldOfStudy = fieldOfStudy;
            Specialization = specialization;
            University = university;
            Term = term;
            IconName = iconName;
            Chats = chats;
        }
    }
}

using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace GOSSIP.ViewModels
{
    public class LogInVM : ObservableObject
    {
        private MainVM _mainVM;

        private string _emailOrUsername;
        public string EmailOrUsername
        {
            get => _emailOrUsername;
            set
            {
                _emailOrUsername = value;
                OnPropertyChanged(nameof(EmailOrUsername));
            }
        }
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event Action<UserModel> RequestClose;

        public ICommand LogInCommand { get; set; }

        public LogInVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            LogInCommand = new RelayCommand(LogInMethod);
        }
        
        private void LogInMethod(object obj)
        {
            UserModel userModel = new(
                 1,
                 "1",
                 "pupsaik",
                 "1",
                 "Student",
                 "IT",
                 "Software Engineering",
                 "Lviv Polytechnic",
                 2,
                 "Bachelor",
                 "User",
                 DateTime.Now,
                 false,
                 "pack://application:,,,/Resources/Images/TempUserIcons/stelmakh_yurii.png",
                 null
                );

            UserModel secondUser = new UserModel(1, "email", "OleksaLviv", "password", "Student", "IT", "Computer Science", "Lviv Polytechnic", 2, "Bachelor", "User", DateTime.Now, false, "pack://application:,,,/Resources/Images/TempUserIcons/OleksaLviv.png", []);

            List<ChatModel> chats = [
                new ChatModel(
                    1,
                    [null, null],
                    "OleksaLviv",
                    DateTime.Now,
                    false,
                    [])
            ];

            List<MessageModel> messageModels = [
                new MessageModel(1, chats[0], null, "привіт", DateTime.Now, false, false),
                new MessageModel(1, chats[0], null, "привіт", DateTime.Now, false, false)
                ];

            chats[0].Messages = messageModels;
            userModel.Chats = chats;

            List<UserModel> list = new() { userModel };

            string jsonString1 = JsonSerializer.Serialize(list);
            File.WriteAllText("user_data.json", jsonString1);

            if (string.IsNullOrEmpty(EmailOrUsername) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            AuthUserModel authUserModel = new();
            if (EmailOrUsername.Contains('@'))
            {
                authUserModel.Email = EmailOrUsername;
            }
            else
            {
                authUserModel.Username = EmailOrUsername;
            }

            authUserModel.Password = Password;

            var jsonString = File.ReadAllText("user_data.json");

            List<UserModel> users = JsonSerializer.Deserialize<List<UserModel>>(jsonString);
            var user = users.Find(u => (u.Username == authUserModel.Username || u.Email == authUserModel.Email) && u.Password == Password);

            if (user == null)
            {
                MessageBox.Show("Incorrect username/email or password");
                return;
            }
            RequestClose.Invoke(user);
        }
    }
}

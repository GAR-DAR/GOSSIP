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

        private bool _isFailed;
        public bool IsFailed
        {
            get => _isFailed;
            set
            {
                _isFailed = value;
                OnPropertyChanged(nameof(IsFailed));
            }
        }

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

        public event Action<UserVM> RequestClose;

        public ICommand LogInCommand { get; set; }

        public LogInVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            LogInCommand = new RelayCommand(LogInMethod);
        }
        
        private void LogInMethod(object obj)
        {
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

            List<UserModel> users = JsonSerializer.Deserialize<List<UserModel>>(jsonString, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve });
            var user = users.Find(u => (u.Username == authUserModel.Username || u.Email == authUserModel.Email) && u.Password == Password);

            if (user == null)
            {
                IsFailed = true;
                return;
            }
            RequestClose.Invoke(new(user));
        }
    }
}

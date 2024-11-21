using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            var jsonString = File.ReadAllText("user_data.json");
            List<UserModel> users = JsonSerializer.Deserialize<List<UserModel>>(jsonString);
            var user = users.Find(u => (u.Username == EmailOrUsername || u.Email == EmailOrUsername) && u.Password == Password);
            if(user == null)
            {
                MessageBox.Show("Incorrect username/email or password");
                return;
            }
            RequestClose.Invoke(user);
        }
    }
}

using GOSSIP.Net;
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
        public ICommand HyperlinkCommand { get; set; }
        public event Action HyperlinkEvent;

        public LogInVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            LogInCommand = new RelayCommand(LogInMethod);
            HyperlinkCommand = new RelayCommand(obj => HyperlinkEvent?.Invoke());
        }

        private void LogInMethod(object obj)
        {
            var authUser = new AuthUserModel
            {
                
                Password = Password
            };

            if (EmailOrUsername.Contains('@'))
            {
                authUser.Email = EmailOrUsername;
                authUser.Username = null;
            }
            else
            {
                authUser.Username = EmailOrUsername;
                authUser.Email = null;
            }
           
            Globals.server.Login(authUser);
            Globals.server.loginEvent += (user) => OnLoginSuccess(user);
        }

        public void OnUserLoggedIn(UserVM user)
        {
            MainVM.AuthorizedUserVM = user;
            _mainVM.SelectedTopBarVM = new TopBarLoggedInVM(_mainVM);
        }

        private void OnLoginSuccess(UserModel user)
        {
            Globals.server.loginEvent -= OnLoginSuccess;

            if (user == null)
            {
				
				IsFailed = true;
                return;
            }
            else
            {
                OnUserLoggedIn(new UserVM(user));
                RequestClose?.Invoke(new UserVM(user));
            }
        }

        public void UpdatePasswordField()
        {
            OnPropertyChanged(nameof(Password));
        }
    }
}

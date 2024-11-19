using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class TopBarLoggedInVM : ObservableObject
    {
        private UserModel _authorizedUser;

        private string _iconPath;
        public string IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                OnPropertyChanged(nameof(IconPath));
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public TopBarLoggedInVM(UserModel userModel)
        {
            _authorizedUser = userModel;
            _username = _authorizedUser.Username;
            IconPath = _authorizedUser.IconPath;
        }
    }
}

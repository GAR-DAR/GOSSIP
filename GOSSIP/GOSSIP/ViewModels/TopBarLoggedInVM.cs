using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class TopBarLoggedInVM : ObservableObject
    {
        private UserModel _authorizedUser;

        public ICommand ProfilePictureClickCommand { get; set; }
        public ICommand ViewProfileCommand { get; set; }

        public event Action ProfileOpeningEvent;

        private bool _isMenuOpen;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set
            {
                _isMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
            }
        }

        private string _photo;
        public string Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged(nameof(Photo));
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

        public TopBarLoggedInVM(UserModel userModel, MainVM mainVM)
        {
            _authorizedUser = userModel;
            MainVM.AuthorizedUser = _authorizedUser;
            _username = _authorizedUser.Username;
            Photo = _authorizedUser.Photo;

            ProfilePictureClickCommand = new RelayCommand((obj) => IsMenuOpen = !IsMenuOpen);
            ViewProfileCommand = new RelayCommand((obj) => { mainVM.OpenProfile(userModel); IsMenuOpen = false; });
        }  
    }
}

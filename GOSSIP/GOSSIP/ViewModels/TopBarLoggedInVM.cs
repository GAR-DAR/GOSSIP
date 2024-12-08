using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class TopBarLoggedInVM : ObservableObject
    {
        public UserVM AuthorizedUserVM { get; set; }

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

        public string Photo
        {
            get => AuthorizedUserVM.Photo;
            set
            {
                AuthorizedUserVM.Photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        public string Username
        {
            get => AuthorizedUserVM.Username;
            set
            {
                AuthorizedUserVM.Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public TopBarLoggedInVM(MainVM mainVM)
        {
            AuthorizedUserVM = MainVM.AuthorizedUserVM;
            Photo = AuthorizedUserVM.Photo;

            ProfilePictureClickCommand = new RelayCommand((obj) => IsMenuOpen = !IsMenuOpen);

            ViewProfileCommand = new RelayCommand((obj) => { mainVM.OpenProfile(MainVM.AuthorizedUserVM); IsMenuOpen = false; });

            AuthorizedUserVM.PropertyChanged += AuthorizedUserVM_PropertyChanged;
        }

        private void AuthorizedUserVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AuthorizedUserVM.Username))
            {
                OnPropertyChanged(nameof(Username));
            }
            if (e.PropertyName == nameof(AuthorizedUserVM.Photo))
            {
                OnPropertyChanged(nameof(Photo));
            }
        }

    }
}

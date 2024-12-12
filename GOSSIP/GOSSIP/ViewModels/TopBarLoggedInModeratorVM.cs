using GOSSIP.Net;
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
    public class TopBarLoggedInModeratorVM : ObservableObject
    {
        public UserVM AuthorizedUserVM { get; set; }

        public ICommand SubmitCommand { get; }
        public ICommand ProfilePictureClickCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand ShowBannedUsersCommand { get; }

        public ICommand LogoutCommand { get; set; }

        private MainVM _mainVM;

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

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        public TopBarLoggedInModeratorVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            AuthorizedUserVM = MainVM.AuthorizedUserVM;
            Photo = AuthorizedUserVM.Photo;

            ProfilePictureClickCommand = new RelayCommand((obj) => IsMenuOpen = !IsMenuOpen);
            ViewProfileCommand = new RelayCommand((obj) => { mainVM.OpenProfile(MainVM.AuthorizedUserVM); IsMenuOpen = false; });
            ShowBannedUsersCommand = new RelayCommand(mainVM.ShowBannedUsersWindow);

            AuthorizedUserVM.PropertyChanged += AuthorizedUserVM_PropertyChanged;

            LogoutCommand = new RelayCommand(LogoutMethod);
            SubmitCommand = new RelayCommand(SubmitMethod);
        }

        private void SubmitMethod(object obj)
        {
            MessageBox.Show("Тут може бути ваша логіка");
        }

        private void LogoutMethod(object obj)
        {
            Globals.server.LogOut();
            _mainVM.Logout();

            IsMenuOpen = false;
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

using GOSSIP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class AuthUserProfileVM : ObservableObject
    {
        public UserVM UserVM { get; set; }

        public AuthUserProfileInfoVM AuthUserProfileInfoVM { get; set; }
        public AuthUserProfileSettingsVM AuthUserProfileSettingsVM { get; set; }

        private bool _isProfileSelected = true;
        public bool IsProfileSelected
        {
            get => _isProfileSelected;
            set
            {
                _isProfileSelected = value;
                OnPropertyChanged(nameof(IsProfileSelected));
            }
        }

        private bool _isSettingsSelected;
        public bool IsSettingsSelected
        {
            get => _isSettingsSelected;
            set
            {
                _isSettingsSelected = value;
                OnPropertyChanged(nameof(IsSettingsSelected));
            }
        }

        private ObservableObject _selectedVM;
        public ObservableObject SelectedVM
        {
            get => _selectedVM;
            set
            {
                _selectedVM = value;
                OnPropertyChanged(nameof(SelectedVM));
            }
        }

        public ICommand BackCommand { get; }
        public ICommand ShowProfileInfoCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        public AuthUserProfileVM(MainVM mainVM)
        {
            UserVM = MainVM.AuthorizedUserVM;
            AuthUserProfileInfoVM = new(UserVM, mainVM);
            AuthUserProfileSettingsVM = new(UserVM, mainVM);

            SelectedVM = AuthUserProfileInfoVM;

            BackCommand = new RelayCommand(obj => mainVM.SwitchToPreviousVM());

            ShowProfileInfoCommand = new RelayCommand(obj =>
            {
                SelectedVM = AuthUserProfileInfoVM;
                IsProfileSelected = true;
                IsSettingsSelected = false;
            });
            ShowSettingsCommand = new RelayCommand(obj =>
            {
                SelectedVM = AuthUserProfileSettingsVM;
                IsProfileSelected = false;
                IsSettingsSelected = true;
            });

        }
    }
}

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

        public ObservableCollection<ObservableObject> AvailableVMs { get; set; }

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

        public AuthUserProfileVM(MainVM mainVM)
        {
            UserVM = MainVM.AuthorizedUserVM;

            AuthUserProfileInfoVM authUserProfileInfoVM = new(UserVM, mainVM);
            AuthUserProfileSettingsVM authUserProfileSettingsVM = new(UserVM, mainVM);
            AvailableVMs = [authUserProfileInfoVM, authUserProfileSettingsVM];

            SelectedVM = authUserProfileInfoVM;

            BackCommand = new RelayCommand(obj => mainVM.SwitchToPreviousVM());
        }
    }
}

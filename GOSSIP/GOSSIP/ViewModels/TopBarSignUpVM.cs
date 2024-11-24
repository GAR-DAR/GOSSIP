using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class TopBarSignUpVM : ObservableObject
    {
        private MainVM _mainVM;

        public ICommand ShowSignUpCommand { get; set; }
        public ICommand ShowLogInCommand { get; set; }

        public TopBarSignUpVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            ShowSignUpCommand = new RelayCommand(_mainVM.ShowSignUpMethod);
            ShowLogInCommand = new RelayCommand(_mainVM.ShowLogInMethod);
        }
    }
}

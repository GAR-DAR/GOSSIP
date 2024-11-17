using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class SignUpSecondVM : ObservableObject
    {
        private readonly SignUpMainVM _mainVM;

        public ICommand BackCommand { get; set; }
        public ICommand NextCommand { get; set; }

        public SignUpSecondVM(SignUpMainVM signUpMainVM)
        {
            _mainVM = signUpMainVM;

            BackCommand = new RelayCommand((obj) => _mainVM.SelectedVM = _mainVM.SignUpFirstVM);
        }
    }
}

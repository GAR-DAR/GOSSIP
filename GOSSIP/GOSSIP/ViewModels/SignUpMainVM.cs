using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class SignUpMainVM : ObservableObject
    {
        private ObservableObject _selectedVM;
        public string Email;
        public string Username;
        public string Password;
        public string RepeatPassword;
        
        public ObservableObject SelectedVM
        {
            get => _selectedVM;
            set
            {
                _selectedVM = value;
                OnPropertyChanged(nameof(SelectedVM));
            }
        }

        public SignUpFirstVM SignUpFirstVM { get; set; }
        public SignUpSecondVM SignUpSecondVM { get; set; }

        public SignUpMainVM()
        {
            SignUpFirstVM = new(this);
            SignUpSecondVM = new(this);
            SelectedVM = new SignUpFirstVM(this); 
        }
    }
}

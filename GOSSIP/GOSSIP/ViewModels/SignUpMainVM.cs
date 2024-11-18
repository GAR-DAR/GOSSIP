using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    //Головний VM реєстрації. Потрібен для збереження всієї введеної інформації для її упорядковування в одному місці
    //для формування об'єкту Користувач
    public class SignUpMainVM : ObservableObject
    {
        private ObservableObject _selectedVM;
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string Status { get; set; }
        public string FieldOfStudy { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public string Degree { get; set; }
        public int Term { get; set; }

        public event Action RequestClose;

        //Переключення VM реєстрації. Зміна поля змінює вид вікна реєстрації
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
            SignUpSecondVM.CloseDialog += RequestCloseMethod;
            SelectedVM = new SignUpFirstVM(this);
        }

        public void RequestCloseMethod()
        {
            RequestClose?.Invoke();
        }
    }
}

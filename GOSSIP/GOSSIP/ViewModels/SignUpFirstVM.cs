using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //VM першого вікна реєстрації. Поля для вводу імейлу, юзернейму, паролю та зміни паролю.
    //Заповнення всіх полів обов'язкове.
    public class SignUpFirstVM : ObservableObject
    {
        //Копія головної моделі реєстрації. Поля будуть заповнюватись одразу в неї
        private readonly SignUpMainVM _mainVM;

        public string Email
        {
            get => _mainVM.Email;
            set
            {
                _mainVM.Email = value;
                OnPropertyChanged(Email);
            }
        }

        public string Username
        {
            get => _mainVM.Username;
            set
            {
                _mainVM.Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _mainVM.Password;
            set
            {
                _mainVM.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string RepeatPassword
        {
            get => _mainVM.RepeatPassword;
            set
            {
                _mainVM.RepeatPassword = value;
                OnPropertyChanged(nameof(RepeatPassword));
            }
        }

        private bool _allFieldsAreNotFilled;
        public bool AllFieldsAreNotFilled
        {
            get => _allFieldsAreNotFilled;
            set
            {
                _allFieldsAreNotFilled = value;
                OnPropertyChanged(nameof(AllFieldsAreNotFilled));
            }
        }

        private bool _passwordsDoNotMatch;
        public bool PasswordsDoNotMatch
        {
            get => _passwordsDoNotMatch;
            set
            {
                _passwordsDoNotMatch = value;
                OnPropertyChanged(nameof(PasswordsDoNotMatch));
            }
        }

        public ICommand NextCommand { get; set; }
        public ICommand HyperlinkCommand { get; set; }
        public event Action HyperlinkEvent;

        public SignUpFirstVM(SignUpMainVM signUpMainVM)
        {
            _mainVM = signUpMainVM;

            NextCommand = new RelayCommand(NextMethod);
            HyperlinkCommand = new RelayCommand(HyperlinkMethod);
        }

        private void HyperlinkMethod(object obj)
        {
            HyperlinkEvent?.Invoke();
        }

        //Команда переключення на наступне вікно. Потребує заповнення всіх полей. Інакше ArgumentException
        private void NextMethod(object obj)
        {
            try
            {
                ValidateInput();
                _mainVM.SelectedVM = _mainVM.SignUpSecondVM;
            }
            catch (ArgumentException ex)
            {
            }
        }

        private void ValidateInput()
        {

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RepeatPassword))
            {
                AllFieldsAreNotFilled = true;
                throw new ArgumentException("Please fill in all fields.");
            }
            else
                AllFieldsAreNotFilled = false;


            if (Password != RepeatPassword)
            {
                PasswordsDoNotMatch = true;
                throw new ArgumentException("Passwords do not match.");
            }
            else
                PasswordsDoNotMatch = false;
        }


    }
}

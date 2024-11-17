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
    public class SignUpFirstVM : ObservableObject
    {
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

        public List<string> StatusOptions { get; set; } = ["Student", "Faculty", "Learner", "None"];
        public List<string> FieldOfStudyOptions { get; set; } = ["Computer ", "Faculty", "Learner", "None"];
        public List<string> SpecializationOptions { get; set; } = ["", "Faculty", "Learner", "None"];



        public ICommand NextCommand { get; set; }

        public SignUpFirstVM(SignUpMainVM signUpMainVM)
        {
            _mainVM = signUpMainVM;

            NextCommand = new RelayCommand(NextMethod);
        }

        private void NextMethod(object obj)
        {
            try
            {
                ValidateInput();
                _mainVM.SelectedVM = _mainVM.SignUpSecondVM;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ValidateInput()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RepeatPassword))
                throw new ArgumentException("Please fill in all fields.");
        }
    }
}

using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

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
            ShowSignUpCommand = new RelayCommand((obj) =>
            {
                SignUpMainVM signUpMainVM = new();
                SignUpWindow signUpView = new() { DataContext = signUpMainVM };
                signUpMainVM.RequestClose += (user) => 
                { 
                    MainVM.AuthorizedUserVM = user;
                    signUpView.Close();
                    _mainVM.ChooseTopBar(user);
                };
                signUpView.ShowDialog();
            });

            ShowLogInCommand = new RelayCommand((obj) =>
            {
                LogInVM logInVM = new(_mainVM);
                LogInWindow logInWindow = new() { DataContext = logInVM };
                logInVM.RequestClose += (user) =>
                {

                    MainVM.AuthorizedUserVM = user;
                     Application.Current.Dispatcher.Invoke(() =>
                    {
                        logInWindow.Close();
                    });
					
                    _mainVM.ChooseTopBar(user);

                };
                logInWindow.ShowDialog();
            });
        }
    }
}

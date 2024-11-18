using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Головна VM. Потрібна для переключення View
    public class MainVM : ObservableObject
    {
        private ObservableObject _selectedVM;

        //Вікна вкладок, що представлені на тулбарі. В майбутньому будуть ще теги
        public ChatsVM ChatsVM = new();
        private PostsListVM PostsListVM = new();

        //Прив'язаний до UI. Його зміна змінить зовнішній вигляд вікна
        public ObservableObject SelectedVM
        {
            get
            {
                return _selectedVM;
            }
            set
            {
                _selectedVM = value;
                OnPropertyChanged(nameof(SelectedVM));
            }
        }

        //Команди переключеня, ініціалізовані в конструкторі
        public ICommand ShowPostsListCommand { get; set; }
        public ICommand ShowChatsCommand { get; set; }
        public ICommand ShowSignUpCommand { get; set; }

        public MainVM()
        { 
            SelectedVM = PostsListVM;
            ShowPostsListCommand = new RelayCommand((obj) => SelectedVM = PostsListVM);
            ShowChatsCommand = new RelayCommand((obj) => SelectedVM = ChatsVM);
            
            //Явне підключення DataContext нового вікна
            ShowSignUpCommand = new RelayCommand((obj) =>
            {
                SignUpWindow signUpView = new() { DataContext = new SignUpMainVM() };
                signUpView.ShowDialog();
            });
        }

    }
}

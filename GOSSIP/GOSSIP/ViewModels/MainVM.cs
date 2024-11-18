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
        private bool _isTopicsPressed = true;
        private bool _isTagsPressed = false;
        private bool _isChatsPressed = false;

        public bool IsTopicsPressed
        {
            get => _isTopicsPressed;
            set
            {
                _isTopicsPressed = value;
                OnPropertyChanged(nameof(IsTopicsPressed));
            }
        }

        public bool IsTagsPressed
        {
            get => _isTagsPressed;
            set
            {
                _isTagsPressed = value;
                OnPropertyChanged(nameof(IsTagsPressed));
            }
        }

        public bool IsChatsPressed
        {
            get => _isChatsPressed;
            set
            {
                _isChatsPressed = value;
                OnPropertyChanged(nameof(IsChatsPressed));
            }
        }

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
            ShowPostsListCommand = new RelayCommand(ShowPostsListMethod);
            ShowChatsCommand = new RelayCommand(ShowChatsMethod);
            
            //Явне підключення DataContext нового вікна
            ShowSignUpCommand = new RelayCommand((obj) =>
            {
                SignUpWindow signUpView = new() { DataContext = new SignUpMainVM() };
                signUpView.ShowDialog();
            });
        }

        private void ShowPostsListMethod(object obj)
        {
            SelectedVM = PostsListVM;
            TurnOffButtonsExcept("Topics");
        }

        private void ShowChatsMethod(object obj)
        {
            SelectedVM = ChatsVM;
            TurnOffButtonsExcept("Chats");
        }

        private void TurnOffButtonsExcept(string button)
        {
            IsTopicsPressed = button == "Topics" ? true : false;
            IsTagsPressed = button == "Tags" ? true : false;
            IsChatsPressed = button == "Chats" ? true : false;
        }
    }
}

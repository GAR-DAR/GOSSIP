using GOSSIP.Models;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public UserModel AuthorizedUser { get; set; }
        
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
        private ObservableObject _selectedTopBarVM;

        //Вікна вкладок, що представлені на тулбарі. В майбутньому будуть ще теги
        public ChatsVM ChatsVM;
        private PostsListVM PostsListVM = new();
        
        public TopBarSignUpVM TopBarSignUpVM { get; set; }

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

        public ObservableObject SelectedTopBarVM
        {
            get
            {
                return _selectedTopBarVM;
            }
            set
            {
                _selectedTopBarVM = value;
                OnPropertyChanged(nameof(SelectedTopBarVM));
            }
        }

        //Команди переключеня, ініціалізовані в конструкторі
        public ICommand ShowPostsListCommand { get; set; }
        public ICommand ShowChatsCommand { get; set; }

        public MainVM()
        {
            TopBarSignUpVM = new(this);
            SelectedVM = PostsListVM;
            SelectedTopBarVM = TopBarSignUpVM;
            ShowPostsListCommand = new RelayCommand(ShowPostsListMethod);
            ShowChatsCommand = new RelayCommand(ShowChatsMethod);            
        }

        private void ShowPostsListMethod(object obj)
        {
            SelectedVM = PostsListVM;
            TurnOffButtonsExcept("Topics");
        }

        private void ShowChatsMethod(object obj)
        {
            if (AuthorizedUser != null)
            {
                if (ChatsVM == null)
                {
                    ChatsVM = new ChatsVM(AuthorizedUser);
                }
                SelectedVM = ChatsVM;
                TurnOffButtonsExcept("Chats");
            }
            else
            {
                MessageBox.Show("Authorize first.");
            }   
        }

        private void TurnOffButtonsExcept(string button)
        {
            IsTopicsPressed = button == "Topics" ? true : false;
            IsTagsPressed = button == "Tags" ? true : false;
            IsChatsPressed = button == "Chats" ? true : false;
        }
    }
}

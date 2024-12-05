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

        public string ChatIcon => IsChatsPressed 
            ? "pack://application:,,,/Resources/Images/MessageDanube.png" 
            : "pack://application:,,,/Resources/Images/Message.png";

        public string TopicsIcon => IsTopicsPressed
            ? "pack://application:,,,/Resources/Images/TopicsDanube.png"
            : "pack://application:,,,/Resources/Images/Topics.png";

         public string TagsIcon => IsTagsPressed
            ? "pack://application:,,,/Resources/Images/TagsDanube.png"
            : "pack://application:,,,/Resources/Images/Tags.png";

        static public UserModel AuthorizedUser { get; set; }
        
        public bool IsTopicsPressed
        {
            get => _isTopicsPressed;
            set
            {
                _isTopicsPressed = value;
                OnPropertyChanged(nameof(IsTopicsPressed));
                OnPropertyChanged(nameof(TopicsIcon));
            }
        }

        public bool IsTagsPressed
        {
            get => _isTagsPressed;
            set
            {
                _isTagsPressed = value;
                OnPropertyChanged(nameof(IsTagsPressed));
                OnPropertyChanged(nameof(TagsIcon));
            }
        }

        public bool IsChatsPressed
        {
            get => _isChatsPressed;
            set
            {
                _isChatsPressed = value;
                OnPropertyChanged(nameof(IsChatsPressed));
                OnPropertyChanged(nameof(ChatIcon));
            }
        }

        private ObservableObject _selectedVM;
        private ObservableObject _selectedTopBarVM;

        //Вікна вкладок, що представлені на тулбарі. В майбутньому будуть ще теги
        private ChatsVM _chatsVM;
        private TopicsListVM _topicListVM;
        private TopBarSignUpVM _topBarSignUpVM { get; set; }

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
        public ICommand ShowTopicsListCommand { get; set; }
        public ICommand ShowChatsCommand { get; set; }
        public ICommand OpenTopicsCommand { get; set; }
        public ICommand CreateTopicCommand { get; set; }

        public MainVM()
        {
            _topicListVM = new(this);
            _topBarSignUpVM = new(this);
            SelectedVM = _topicListVM;
            SelectedTopBarVM = _topBarSignUpVM;
            ShowTopicsListCommand = new RelayCommand(ShowPostsListMethod);
            ShowChatsCommand = new RelayCommand(ShowChatsMethod);
            CreateTopicCommand = new RelayCommand(CreateTopicMethod);
        }

        private void CreateTopicMethod(object obj)
        {
            if (AuthorizedUser != null)
            {
                SelectedVM = new CreateTopicVM(this);
            }
            else
            {
                ShowLogInMethod(null);
            }
        }

        public void ShowPostsListMethod(object obj)
        {
            SelectedVM = _topicListVM;
            TurnOffButtonsExcept("Topics");
        }

        private void ShowChatsMethod(object obj)
        {
            if (AuthorizedUser != null)
            {
                if (_chatsVM == null)
                {
                    _chatsVM = new ChatsVM(AuthorizedUser, this);
                }
                SelectedVM = _chatsVM;
                TurnOffButtonsExcept("Chats");
            }
            else
            {
                ShowLogInMethod(null);
            }   
        }

        private void TurnOffButtonsExcept(string button)
        {
            IsTopicsPressed = button == "Topics" ? true : false;
            IsTagsPressed = button == "Tags" ? true : false;
            IsChatsPressed = button == "Chats" ? true : false;
        }

        public void ShowSignUpMethod(object obj)
        {
            SignUpMainVM signUpMainVM = new();
                SignUpWindow signUpView = new() { DataContext = signUpMainVM };
                signUpMainVM.RequestClose += (user) => 
                { 
                    AuthorizedUser = user;
                    signUpView.Close();
                    SelectedTopBarVM = new TopBarLoggedInVM(AuthorizedUser, this);
                };
                signUpView.ShowDialog();
        }

        public void ShowLogInMethod(object obj)
        {
            LogInVM logInVM = new(this);
            LogInWindow logInWindow = new() { DataContext = logInVM };
            logInVM.RequestClose += (user) =>
            {
                AuthorizedUser = user;
                logInWindow.Close();
                SelectedTopBarVM = new TopBarLoggedInVM(AuthorizedUser, this);
            };
            logInWindow.ShowDialog();
        }
    }
}

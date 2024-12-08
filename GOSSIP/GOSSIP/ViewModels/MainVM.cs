using GOSSIP.Models;
using GOSSIP.Net;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        static public UserVM AuthorizedUserVM { get; set; }

        public List<ObservableObject> StackOfVMs { get; set; } = [];
        
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
                Globals.server.SendPacket(SignalsEnum.RefreshUser, AuthorizedUser);
                Globals.server.refreshUserEvent += (user) =>
                {
                    AuthorizedUser = user;
                    //TopBarLoggedInVM topBarLoggedInVM = new(AuthorizedUser, this);
                    //SelectedTopBarVM = topBarLoggedInVM;
                    OnPropertyChanged(nameof(AuthorizedUser));
                };

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
            StackOfVMs.Add(_topicListVM);
        }

        private void CreateTopicMethod(object obj)
        {
            if (AuthorizedUserVM != null)
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
            StackOfVMs.Add(SelectedVM);
            _topicListVM.UpdateInfo();
            SelectedVM = _topicListVM;
            StackOfVMs.RemoveAt(StackOfVMs.Count - 1);
            TurnOffButtonsExcept("Topics");
        }

        private void ShowChatsMethod(object obj)
        {
            if (AuthorizedUserVM != null)
            {
                if (_chatsVM == null)
                {
                    _chatsVM = new ChatsVM(AuthorizedUserVM.UserModel, this);
                }
                StackOfVMs.Add(SelectedVM);
                SelectedVM = _chatsVM;
                TurnOffButtonsExcept("Chats");
            }
            else
            {
                ShowLogInMethod(null);
            }   
        }

        public void OpenTopic(TopicVM topiVM)
        {
            OpenedTopicVM openedTopicVM = new(topiVM, this, _topicListVM);
            StackOfVMs.Add(SelectedVM);
            SelectedVM = openedTopicVM;
        }

        private void TurnOffButtonsExcept(string button)
        {
            IsTopicsPressed = button == "Topics" ? true : false;
            IsTagsPressed = button == "Tags" ? true : false;
            IsChatsPressed = button == "Chats" ? true : false;
        }

        public void SwitchToPreviousVM()
        {
            if (StackOfVMs.Last() is TopicsListVM)
            {
                ShowPostsListMethod(null);
            }
            else
            {
                StackOfVMs.RemoveAt(StackOfVMs.Count - 1);
                SelectedVM = StackOfVMs.Last();
            }
        }

        public void OpenProfile(UserVM user)
        {
            ObservableObject profileVM = AuthorizedUserVM != null && AuthorizedUserVM.UserModel.ID == user.UserModel.ID ? new AuthUserProfileVM(this) : new ProfileVM(this, user);
            SelectedVM = profileVM;
            StackOfVMs.Add(profileVM);
        }

        public void ShowSignUpMethod(object obj)
        {
            SignUpMainVM signUpMainVM = new();
                SignUpWindow signUpView = new() { DataContext = signUpMainVM };
                signUpMainVM.RequestClose += (user) => 
                { 
                    AuthorizedUserVM = user;
                    signUpView.Close();
                    SelectedTopBarVM = new TopBarLoggedInVM(this);
                };
                signUpView.ShowDialog();
        }

        public void ShowLogInMethod(object obj)
        {
            LogInVM logInVM = new(this);
            LogInWindow logInWindow = new() { DataContext = logInVM };
            logInVM.RequestClose += (user) =>
            {
                AuthorizedUserVM = user;
                logInWindow.Close();
                TopBarLoggedInVM topBarLoggedInVM = new(this);
                SelectedTopBarVM = topBarLoggedInVM;
            };
            logInWindow.ShowDialog();
        }

       
    }
}

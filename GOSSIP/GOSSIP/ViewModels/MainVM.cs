using GOSSIP.Models;
using GOSSIP.Net;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                if(AuthorizedUserVM != null)
                {
                    Globals.server.SendPacket(SignalsEnum.RefreshUser, AuthorizedUserVM.UserModel);
                    OnPropertyChanged(nameof(AuthorizedUserVM));
                }

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
                if (AuthorizedUserVM != null)
                {
                    Globals.server.SendPacket(SignalsEnum.RefreshUser, AuthorizedUserVM.UserModel);
                    OnPropertyChanged(nameof(AuthorizedUserVM));
                }

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
                if (AuthorizedUserVM != null)
                {
                    Globals.server.SendPacket(SignalsEnum.RefreshUser, AuthorizedUserVM.UserModel);
                    OnPropertyChanged(nameof(AuthorizedUserVM));
                }
                
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

            if(AuthorizedUserVM != null)
            {
                SelectedTopBarVM = new TopBarLoggedInVM(this);
            }
            else
            {
                SelectedTopBarVM = _topBarSignUpVM;
            }

            ShowTopicsListCommand = new RelayCommand(ShowPostsListMethod);
            ShowChatsCommand = new RelayCommand(ShowChatsMethod);
            CreateTopicCommand = new RelayCommand(CreateTopicMethod);
            StackOfVMs.Add(_topicListVM);

            Globals.server.refreshUserEvent += OnRefreshUser;
            Globals.server.multicastMessageEvent += OnMulticastMessage;
        }

        private void OnRefreshUser(UserModel user)
        {
            if (AuthorizedUserVM != null)
            {
                AuthorizedUserVM.UserModel = user;
                OnPropertyChanged(nameof(AuthorizedUserVM));
                OnPropertyChanged(nameof(AuthorizedUserVM.Username));
                OnPropertyChanged(nameof(AuthorizedUserVM.Photo));
            }
        }

        private void OnMulticastMessage(MessageModel message)
        {
            if (AuthorizedUserVM != null)
            {
                AuthorizedUserVM.UserModel.Chats.Find(chat => chat.ID == message.Chat.ID).Messages.Add(message);

                OnPropertyChanged(nameof(AuthorizedUserVM));
                OnPropertyChanged(nameof(AuthorizedUserVM.UserModel.Chats));
                
            }
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
            //_topicListVM.UpdateInfo();
            SelectedVM = _topicListVM;
            StackOfVMs.RemoveAt(StackOfVMs.Count - 1);
            TurnOffButtonsExcept("Topics");
        }

        private void ShowChatsMethod(object obj)
        {
            if (AuthorizedUserVM != null)
            {
                //UserModel user2 = new(
                //    8,
                //    "email",
                //    "123",
                //    "123",
                //    "Student",
                //    "idgaf",
                //    "idgaf",
                //    "elenu",
                //    4,
                //    "PhD",
                //    "User",
                //    DateTime.Now,
                //    false,
                //    "pack://application:,,,/Resources/Images/TempUserIcons/nophotoicon.png",
                //    []);

                //UserModel user = new(
                //    8,
                //    "email",
                //    "12",
                //    "12",
                //    "Student",
                //    "idgaf",
                //    "idgaf",
                //    "elenu",
                //    4,
                //    "PhD",
                //    "User",
                //    DateTime.Now,
                //    false,
                //    "pack://application:,,,/Resources/Images/TempUserIcons/nophotoicon.png",
                //    []);

                //ChatModel chat = new(
                //    7,
                //    [user, user2],
                //    "12, 123",
                //    DateTime.Now,
                //    false,
                //    []);

                //MessageModel messageModel = new(
                //    4,
                //    chat,
                //    user2,
                //    "Hello",
                //    DateTime.Now,
                //    false,
                //    false
                //    );

                //chat.Messages.Add(messageModel);

                //user.Chats.Add(chat);

                //string json = JsonSerializer.Serialize(new List<UserModel>() { user }, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve });
                //File.WriteAllText("user_data.json", json);


                if (_chatsVM == null)
                {
                    _chatsVM = new ChatsVM(this);
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
            if(profileVM is AuthUserProfileInfoVM && StackOfVMs.Last() != profileVM)
                StackOfVMs.Add(profileVM);
        }

        public void ShowSignUpMethod(object obj)
        {
            SignUpMainVM signUpMainVM = new();
                SignUpWindow signUpView = new() { DataContext = signUpMainVM };
                signUpMainVM.RequestClose += (user) => 
                { 
                    AuthorizedUserVM = user;
                    signUpView.Dispatcher.Invoke(() =>
                    {
                        signUpView.Close();
                    }); 
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

                ChooseTopBar(user);
				
                logInWindow.Dispatcher.Invoke(() =>
                {
                    logInWindow.Close();
                });
            };
            logInWindow.ShowDialog();
        }


        public void Logout()
        {
            AuthorizedUserVM = null;
            SelectedTopBarVM = _topBarSignUpVM;
            SelectedVM = _topicListVM;

            // Clear the stack of VMs if necessary
            StackOfVMs.Clear();
            StackOfVMs.Add(_topicListVM);

            // Optionally, reset the state of buttons
            TurnOffButtonsExcept("Topics");

            // Notify property changes if needed
            OnPropertyChanged(nameof(AuthorizedUserVM));
        }

        public void ChooseTopBar(UserVM user)
        {
            _ = AuthorizedUserVM.Role == "Moderator" ? SelectedTopBarVM = new TopBarLoggedInModeratorVM(this) : SelectedTopBarVM = new TopBarLoggedInVM(this);
        }

        public void ShowBannedUsersWindow(object obj)
        {
            BannedUsersVM bannedUsersVM = new();
            BannedUsersWindow bannedUsersWindow = new() { DataContext = bannedUsersVM };
            bannedUsersVM.CloseCommand = new RelayCommand(obj => bannedUsersWindow.Close());
            bannedUsersWindow.ShowDialog();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GOSSIP.Net;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class AddUsersToChatVM : ObservableObject
    {

        public AddUsersToChatVM()
        {

            AllUsers = new (Globals.AllUsers_Cache.Select(x => new UserVM(x)).ToList());
            
            CreateChatCommand = new RelayCommand(CreateChatMethod);
            CloseCommand = new RelayCommand(obj => RequestClose?.Invoke(false));
        }

        public ICommand CloseCommand { get; set; }
        public ICommand CreateChatCommand { get; }

        // Подія для запиту на закриття вікна
        public event Action<bool?> RequestClose;


    private void getUsers(List<UserModel> users)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            AllUsers = new ObservableCollection<UserVM>(
                users.Where(x => x.ID != MainVM.AuthorizedUserVM.UserModel.ID)
                     .Select(x => new UserVM(x)));
            OnPropertyChanged(nameof(AllUsers));
        });
    }

        private void CreateChatMethod(object obj)
        {
            if (SelectedUsers.Count == 0)
            {
                MessageBox.Show("Select at least one user to create a chat");
                return;
            }

            string nameOfChat = MainVM.AuthorizedUserVM.Username + ", " + string.Join(", ", SelectedUsers.Select(user => user.Username));
            if (nameOfChat.Length > 40)
            {
                nameOfChat = nameOfChat.Substring(0, 40);
            }

            List<UserModel> users = [MainVM.AuthorizedUserVM.UserModel];
            users.AddRange(SelectedUsers.Select(x => x.UserModel));

            ChatModel newChat = new(
                0,
                users,
                nameOfChat,
                DateTime.Now,
                false,
                []
            );

            Globals.server.SendPacket(SignalsEnum.StartChat, new ChatModelID(newChat));

            MainVM.AuthorizedUserVM.UserModel.Chats.Add(newChat);
            RequestClose?.Invoke(true);
        }


        private ObservableCollection<UserVM> _allUsers;
        public ObservableCollection<UserVM> AllUsers 
        { 
            get => _allUsers;
            set
            {
                _allUsers = value;
                OnPropertyChanged(nameof(AllUsers));
            }
        }
        public ObservableCollection<UserVM> FilteredUsers { get; set; } = new();
        public List<UserVM> SelectedUsers { get; set; } = new();
		
	
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterUsers();
                }
            }
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                if (_isPopupOpen != value)
                {
                    _isPopupOpen = value;
                    OnPropertyChanged(nameof(IsPopupOpen));
                }
            }
        }

        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredUsers = new ObservableCollection<UserVM>();
                IsPopupOpen = false;
            }
            else
            {
                var lowerSearchText = SearchText.ToLower();
                FilteredUsers = new ObservableCollection<UserVM>(
                    AllUsers.Where(user => user.Username.ToLower().Contains(lowerSearchText)));

                IsPopupOpen = FilteredUsers.Any();
            }
            OnPropertyChanged(nameof(FilteredUsers));
        }
    }
}

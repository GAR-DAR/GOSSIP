using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class AddUsersToChatVM : ObservableObject
    {
        public ICommand CloseCommand { get; set; }
        public ICommand CreateChatCommand { get; }

        // Подія для запиту на закриття вікна
        public event Action<bool?> RequestClose;

        public ObservableCollection<UserVM> AllUsers { get; set; }
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

        public AddUsersToChatVM()
        {
            JsonStorage jsonStorage = new("user_data.json");
            AllUsers = new ObservableCollection<UserVM>(
                jsonStorage.LoadUsers()
                           .Select(x => new UserVM(x))
                           .Where(x => x.UserModel.ID != MainVM.AuthorizedUserVM.UserModel.ID));

            CreateChatCommand = new RelayCommand(CreateChatMethod);
            CloseCommand = new RelayCommand(obj => RequestClose?.Invoke(false));
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

            List<UserModel> users = [MainVM.AuthorizedUserVM.UserModel, .. SelectedUsers.Select(x => x.UserModel)];

            ChatModel newChat = new(
                0,
                users,
                nameOfChat,
                DateTime.Now,
                false,
                new List<MessageModel>()
            );

            MainVM.AuthorizedUserVM.UserModel.Chats.Add(newChat);
            RequestClose?.Invoke(true);
        }
    }
}

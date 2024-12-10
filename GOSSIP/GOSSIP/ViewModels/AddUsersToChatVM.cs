using GOSSIP.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GOSSIP.Net;
using System.Windows.Input;
using GOSSIP.Models.IDModels;


namespace GOSSIP.ViewModels
{
    public class AddUsersToChatVM : ObservableObject
    {

        public AddUsersToChatVM()
        {
            Globals.server.getUsersEvent += getUsers;
            Globals.server.GetAllUsers();

            CreateChatCommand = new RelayCommand(CreateChatMethod);
            CloseCommand = new RelayCommand(obj => RequestClose?.Invoke(false));
        }

        public ICommand CloseCommand { get; set; }
        public ICommand CreateChatCommand { get; }

        // Подія для запиту на закриття вікна
        public event Action<bool?> RequestClose;


    private void getUsers(List<UserModelID> users)
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

            List<UserModelID> users = [MainVM.AuthorizedUserVM.UserModel];
            users.AddRange(SelectedUsers.Select(x => x.UserModel));

            ChatModelID newChat = new(
                0,
                users,
                nameOfChat,
                DateTime.Now,
                false,
                []
                );

            Globals.server.SendPacket(SignalsEnum.StartChat, newChat);
            Globals.server.SendPacket(SignalsEnum.RefreshUser, MainVM.AuthorizedUserVM.UserModel);
            MainVM.AuthorizedUserVM.UserModel.ChatsID.Add(newChat);
            RequestClose?.Invoke(true);
        }


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

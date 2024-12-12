using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GOSSIP.Net;
using System.Timers;
using System.ComponentModel;

namespace GOSSIP.ViewModels
{
    //Список чатів, представлених зліва екрану чатік
    public class ChatsVM : ObservableObject
    {
        //Прив'язана до UI.
        private ObservableCollection<OpenedChatVM> _chatList = [];
        public ObservableCollection<OpenedChatVM> ChatList
        { 
            get => _chatList;
            set
            {
                _chatList = value;
                OnPropertyChanged(nameof(ChatList));
            }
        }
        public uint CurrentUserID { get; set; }

        private readonly MainVM _mainVM;

        private OpenedChatVM _openedChatVM;
        
        public ICommand AddNewChatCommand { get; }

        //Визначає, який чат відкритий
        public OpenedChatVM OpenedChatVM
        {
            get => _openedChatVM;
            set
            {
                _openedChatVM = value;

                OnPropertyChanged(nameof(MainVM.AuthorizedUserVM.UserModel));

                OnPropertyChanged(nameof(OpenedChatVM));
            }
        }

        private void AddNewChatMethod(object obj)
        {
            AddUsersToChatVM addUsersToChatVM = new();
            AddUsersToChatWindow addUsersToChatWindow = new() { DataContext = addUsersToChatVM };

            // Підписуємось на подію закриття
            addUsersToChatVM.RequestClose += result =>
            {
                addUsersToChatWindow.DialogResult = result;
                addUsersToChatWindow.Close();
            };

            // Відкриваємо діалогове вікно
            bool? dialogResult = addUsersToChatWindow.ShowDialog();

            //// Обробляємо результат
            //if (dialogResult == true)
            //{
            //    UpdateChats();
            //}
        }



        public ChatsVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            UpdateChats();

            CurrentUserID = MainVM.AuthorizedUserVM.UserModel.ID;
            AddNewChatCommand = new RelayCommand(AddNewChatMethod);
            Globals.server.openChatsEvent += (obj) => { 
                ChatList = new(Globals.User_Cache.Chats.Select(chat => new OpenedChatVM(chat, _mainVM))); 
            };

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UpdateChats();
        }

        public void UpdateChats()
        {
            if(MainVM.AuthorizedUserVM.UserModel.Chats != null)
            {
                ChatList.Clear();
                foreach (ChatModel chatModel in MainVM.AuthorizedUserVM.UserModel.Chats)
                    ChatList.Add(new(chatModel, _mainVM));
            }
        }
    }
}

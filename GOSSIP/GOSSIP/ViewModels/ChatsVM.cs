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
using GOSSIP.Models.IDModels;

namespace GOSSIP.ViewModels
{
    //Список чатів, представлених зліва екрану чатік
    public class ChatsVM : ObservableObject
    {
        //Прив'язана до UI.
        public ObservableCollection<OpenedChatVM> ChatList { get; set; } = [];
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

                Globals.server.SendPacket(SignalsEnum.RefreshUser, MainVM.AuthorizedUserVM.UserModel);
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

            // Обробляємо результат
            if (dialogResult == true)
            {
                UpdateChats();
            }
        }



        public ChatsVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            UpdateChats();

            CurrentUserID = MainVM.AuthorizedUserVM.UserModel.ID;
            AddNewChatCommand = new RelayCommand(AddNewChatMethod);

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UpdateChats();
        }

        public void UpdateChats()
        {
            if(MainVM.AuthorizedUserVM.UserModel.ChatsID != null)
            {
                ChatList.Clear();
                foreach (ChatModelID chatModel in MainVM.AuthorizedUserVM.UserModel.ChatsID)
                    ChatList.Add(new(chatModel, _mainVM));
            }
        }
    }
}

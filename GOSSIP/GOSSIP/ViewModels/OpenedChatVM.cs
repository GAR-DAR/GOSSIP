using GOSSIP.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Інформація про відкритий чат.
    public class OpenedChatVM : ObservableObject
    {
        public MainVM MainVM { get; set; }

        private string _lastMessage;
        private string _enteredText = "";

        //Містить копію моделі чатів
        private ChatModel _chat;

        public string ChatName { get; set; }
        public string Photo { get; set; }

        //Список повідомлень, прив'язаних до UI. Також, прив'язаний до списку чатів моделі ChatModel
        public ObservableCollection<MessageModel> Messages { get; set; }

        
        public OpenedChatVM(ChatModel chat, MainVM mainVM)
        {
            MainVM = mainVM;
            _chat = chat;

            ChatName = chat.Name; //Поки хардкод
            Messages = new(chat.Messages);

            if(Messages.Count != 0)
                LastMessage = Messages.Last().MessageText;

            Photo = chat.Users[0].Photo;

            MainVM.AuthorizedUserVM.PropertyChanged += AuthorizedUserVM_PropertyChanged;

            Messages.CollectionChanged += LastMessageUpdate;
            SendMessageCommand = new RelayCommand(SendMessageMethod);
        }

        private void AuthorizedUserVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainVM.AuthorizedUserVM.UserModel.Chats))
            {
                UpdateMessages();
            }
        }

        private void UpdateMessages()
        {
            var updatedChat = MainVM.AuthorizedUserVM.UserModel.Chats.FirstOrDefault(c => c.ID == _chat.ID);

            if (updatedChat != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Clear();
                });

                foreach (var message in updatedChat.Messages)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.Add(message);
                    });
                }
            }
        }

        public string LastMessage
        {
            get => _lastMessage;
            set
            {
                _lastMessage = value;
                OnPropertyChanged(nameof(LastMessage));
            }
        }

        public string EnteredText
        {
            get => _enteredText;
            set
            {
                _enteredText = value;
                OnPropertyChanged(nameof(EnteredText));
            }
        }

        public ICommand SendMessageCommand { get; set; }

        private void SendMessageMethod(object obj)
        {
            MessageModel message = new MessageModel(1, _chat, MainVM.AuthorizedUserVM.UserModel, this.EnteredText, DateTime.Now, false, false);
            _chat.AddMessage(message);
            Messages.Add(message);
            EnteredText = "";


            Globals.server.SendPacket(SignalsEnum.SendMessage, new MessageModelID(message));
        }

        private void LastMessageUpdate(object sender, EventArgs args)
        {
            if(Messages.Count > 0)
            {
                LastMessage = Messages.Last().MessageText;
            }
        }
    }
}

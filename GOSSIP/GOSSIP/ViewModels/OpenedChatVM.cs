using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ChatService _chatService = new("user_data.json");

        public string ChatName { get; set; }
        public string Photo { get; set; }

        //Список повідомлень, прив'язаних до UI. Також, прив'язаний до списку чатів моделі ChatModel
        public ObservableCollection<MessageModel> Messages { get; set; }

        
        public OpenedChatVM(ChatModel chat, MainVM mainVM)
        {
            MainVM = mainVM;
            _chat = chat;
            ChatName = "OleksaLviv"; //Поки хардкод
            Messages = new(chat.Messages);
            LastMessage = Messages.Last().MessageText;

            //IconPath = _chat.Users[0].Photo;


            //Підписка на зміну колекції з моделі. Модель міняється — міняється UI
            Messages.CollectionChanged += LastMessageUpdate;
            SendMessageCommand = new RelayCommand(SendMessageMethod);
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
        }

        private void LastMessageUpdate(object sender, EventArgs args)
        {
            LastMessage = Messages.Last().MessageText;
        }
    }
}

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
        private string _lastMessage;
        private string _enteredText;

        //Містить копію моделі чатів
        private ChatModel _chat;

        private ChatService _chatService = new("user_data.json");

        public string ChatName { get; set; }
        public string IconPath { get; set; }

        //Список повідомлень, прив'язаних до UI. Також, прив'язаний до списку чатів моделі ChatModel
        public ObservableCollection<MessageModel> Messages { get; set; }

        
        public OpenedChatVM(ChatModel chat)
        {
            _chat = chat;
            ChatName = chat.Interlocutor.Username;
            Messages = new(chat.Messages);
            LastMessage = Messages.Last().MessageText;
            IconPath = _chat.Interlocutor.IconPath;


            //Підписка на зміну колекції з моделі. Модель міняється — міняється UI
            _chat.Messages.CollectionChanged += Messages_CollectionChanged;

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

        //Метод надсилання повідомлення. Напряму міняється колекція з моделі, через підписку на івент, міняється і UI
        private void SendMessageMethod(object obj)
        {
            _chat.AddMessage(new MessageModel(1, _chat.ID, 1, true, EnteredText, DateTime.Now, false, false));
            EnteredText = "";
        }

        //Те, що буде відбуватись під час зміни списку повідомлень моделей
        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MessageModel newMessage in e.NewItems)
                {
                    _chatService.AddMessage(_chat.ID, newMessage);
                    Messages.Add(newMessage);
                    LastMessage = Messages.Last().MessageText;
                }
            }

            if (e.OldItems != null)
            {
                foreach (MessageModel oldMessage in e.OldItems)
                {
                    Messages.Remove(oldMessage);
                    LastMessage = Messages.Last().MessageText;
                }
            }
        }
    }
}

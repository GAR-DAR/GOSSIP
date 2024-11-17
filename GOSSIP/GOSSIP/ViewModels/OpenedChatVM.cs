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
    //Info about opened chat. Connected to ChatModel
    public class OpenedChatVM : ObservableObject
    {
        private string _lastMessage;
        private string _enteredText;
        private ChatModel _chat;

        public string ChatName { get; set; }
        public string IconPath => _chat.Interlocutor.IconPath;
        public ObservableCollection<MessageModel> Messages { get; set; }

        public OpenedChatVM(ChatModel chat)
        {
            _chat = chat;
            ChatName = chat.Interlocutor.Username;
            Messages = new(chat.Messages);
            LastMessage = Messages.Last().MessageText;
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


        //First, VM list of messages is changed. Then it passes a signal to model list and it gets changed
        private void SendMessageMethod(object obj)
        {
            _chat.AddMessage(new MessageModel(1, _chat.ID, 1, true, EnteredText, DateTime.Now, false, false));
            EnteredText = "";
        }

        //Signal described above
        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MessageModel newMessage in e.NewItems)
                {
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

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
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{_chat.IconName}";
        public ObservableCollection<MessageModel> Messages { get; set; }

        public OpenedChatVM(ChatModel chat)
        {
            _chat = chat;
            ChatName = chat.Name;
            Messages = new(chat.Messages);
            LastMessage = Messages.Last().MessageText;
            Messages.CollectionChanged += Messages_CollectionChanged;

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
            Messages.Add(new MessageModel(1, _chat.ID, 1, true, EnteredText, DateTime.Now, false, false));
            EnteredText = "";
        }

        //Signal described above
        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MessageModel newMessage in e.NewItems)
                {
                    _chat.Messages.Add(newMessage);
                    LastMessage = Messages.Last().MessageText;
                }
            }

            if (e.OldItems != null)
            {
                foreach (MessageModel oldMessage in e.OldItems)
                {
                    _chat.Messages.Remove(oldMessage);
                    LastMessage = Messages.Last().MessageText;
                }
            }
        }
    }
}

using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class OpenedChatVM : ObservableObject
    {
        private string _enteredText;
        private ChatModel _chat;

        public string ChatName { get; set; }
        public string LastMessage { get; set; }
        public string IconPath => $"pack://application:,,,/Resources/Images/TempUserIcons/{_chat.IconName}";
        public MessagesListVM Messages { get; set; }

        public OpenedChatVM(ChatModel chat)
        {
            _chat = chat;
            LastMessage = chat.LastMessage;
            ChatName = chat.Name;
            Messages = new(chat.Messages);
            SendMessageCommand = new RelayCommand(SendMessageMethod);
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
            Messages.AddMessage(new MessageModel(1, _chat.ID, 1, true, EnteredText, DateTime.Now, false, false));
            EnteredText = "";
        }
    }
}

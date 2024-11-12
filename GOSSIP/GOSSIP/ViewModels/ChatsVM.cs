using GOSSIP.Models;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class ChatsVM : ObservableObject
    {
        public ObservableCollection<OpenedChatVM> ChatList { get; set; }

        private OpenedChatVM _openedChatVM;
        public OpenedChatVM OpenedChatVM
        {
            get => _openedChatVM;
            set
            {
                _openedChatVM = value;
                OnPropertyChanged(nameof(OpenedChatVM));
            }
        }

        public ChatsVM()
        {
            ChatList =
            [
                new OpenedChatVM(new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png", new ObservableCollection<MessageModel>(){

                    new MessageModel("OleksaLviv", false, "хєхє", new TimeSpan(4, 2, 0)),
                    new MessageModel("OleksaLviv", true, "привіт", new TimeSpan(4, 2, 0))

                    })),
                new OpenedChatVM(new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png", [])),
                //new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png"),
                //new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png"),
                //new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png"),
                //new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png"),
                //new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png"),
                //new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png"),
                //new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png"),
                //new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png"),
                //new ChatModel("OleksaLviv", "я був на вечірці підіді і я маю що сказати", "OleksaLviv.png"),
                //new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png"),
            ];
        }
    }
}

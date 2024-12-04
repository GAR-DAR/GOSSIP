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
    //Список чатів, представлених зліва екрану чатік
    public class ChatsVM : ObservableObject
    {
        //Прив'язана до UI.
        public ObservableCollection<OpenedChatVM> ChatList { get; set; } = [];
        public uint CurrentUserID { get; set; }

        private OpenedChatVM _openedChatVM;
        
        //Визначає, який чат відкритий
        public OpenedChatVM OpenedChatVM
        {
            get => _openedChatVM;
            set
            {
                _openedChatVM = value;
                OnPropertyChanged(nameof(OpenedChatVM));
            }
        }

        public ChatsVM(UserModel currentUser, MainVM mainVM)
        {
            if(currentUser.Chats != null)
            {
                foreach (ChatModel chatModel in currentUser.Chats)
                ChatList.Add(new(chatModel, mainVM));
            }
            
            CurrentUserID = currentUser.ID;

            /*ChatList = chatlist;
            //Захардкоджені чати
            ChatList =
            [
                new OpenedChatVM(new ChatModel(1, DateTime.Now, false, 
                new User("OleksaLviv", "OleksaLviv.png"),
                [

                    new MessageModel(1, 1, 1, false, "хєхє", DateTime.Now,  true, false),
                    new MessageModel(2, 1, 2, true, "привіт", DateTime.Now, true, false),
                    new MessageModel(2, 1, 2, true, "привіт", DateTime.Now, true, false),

                    ])),
                //new OpenedChatVM(new ChatModel("stelmakh_yurii", "ненавиджу ОС ♥", "stelmakh_yurii.png", [])),
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
            ];*/
        }
    }
}

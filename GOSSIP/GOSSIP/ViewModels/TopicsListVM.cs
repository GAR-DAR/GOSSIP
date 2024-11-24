using GOSSIP.Models;
using GOSSIP.Views;
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
    //Список постів. Поки сиро і плачевно
    public class TopicsListVM : ObservableObject
    {
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<TopicModel> Topics { get; set; }

        private MainVM _mainVM;

        private TopicModel _selectedTopic;
        public TopicModel SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
            }
        }

        public ICommand DoubleClickCommand { get; }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));

            Topics =
            [
                new TopicModel(
                    new UserModel(
                        "stelmakh.yurii@example.com",
                        "stelmakh_yurii",
                        "password123",
                        "active",
                        "Computer Science",
                        "Software Development",
                        "Lviv Polytechnic",
                        2,
                        "stelmakh_yurii.png",
                        new List<ChatModel>()
                    ),
                    DateTime.Now.AddMinutes(-10),
                    "What C# GUI framework for desktop apps is the best?",
                    "filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text.",
                    new List<string> { "C#", "GUI" },
                    2,
                    2,
                    new List<ReplyModel>
                    {
                        new ReplyModel(
                            1,
                            new UserModel(
                                "john.doe@example.com",
                                "OleksaLviv",
                                "password456",
                                "active",
                                "IT",
                                "Web Development",
                                "Harvard University",
                                5,
                                "OleksaLviv.png",
                                new List<ChatModel>()
                            ),
                            null, // Topic will be set below
                            null,
                            "I LOVE HER LACK OF ENERGY! GO GIRL, GIVE US NOTHING!",
                            DateTime.Now.AddMinutes(-8),
                            1,
                            false
                        ),
                        new ReplyModel(
                            2,
                            new UserModel(
                                "jane.smith@example.com",
                                "stelmakh_yurii",
                                "password789",
                                "active",
                                "Engineering",
                                "Embedded Systems",
                                "MIT",
                                4,
                                "stelmakh_yurii.png",
                                new List<ChatModel>()
                            ),
                            null, // Topic will be set below
                            null,
                            "stfu hoe",
                            DateTime.Now.AddMinutes(-5),
                            5,
                            false
                        ),
                    }
                )
            ];

            var topic = Topics[0];
            foreach (var reply in topic.Replies)
            {
                reply.Topic = topic;
            }
        }

        private void OnItemDoubleClickedMethod(TopicModel post)
        {
            if (post != null)
            {
                _mainVM.SelectedVM = new OpenedTopicVM(post, _mainVM);
            }
        }
    }
}

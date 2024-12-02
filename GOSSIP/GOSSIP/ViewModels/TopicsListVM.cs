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
                    1,
                    new UserModel
                    {
                        Email = "stelmakh.yurii@example.com",
                        Username = "stelmakh_yurii",
                        Password = "password123",
                        Status = "active",
                        FieldOfStudy = "Computer Science",
                        Specialization = "Software Development",
                        University = "Lviv Polytechnic",
                        Term = 2,
                        Photo = "pack://application:,,,/Resources/Images/TempUserIcons/stelmakh_yurii.png",
                        Chats = new List<ChatModel>()
                    },
                    "What C# GUI framework for desktop apps is the best?",
                    "filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text, filler text.",
                    DateTime.Now.AddMinutes(-10),
                    2,
                    new List<string> { "C#", "GUI" },
                    new List<ReplyModel>
                    {
                        new ReplyModel(
                            1,
                            new UserModel
                            {
                                Email = "john.doe@example.com",
                                Username = "OleksaLviv",
                                Password = "password456",
                                Status = "active",
                                FieldOfStudy = "IT",
                                Specialization = "Web Development",
                                University = "Harvard University",
                                Term = 5,
                                Photo = "pack://application:,,,/Resources/Images/TempUserIcons/OleksaLviv.png",
                                Chats = new List<ChatModel>()
                            },
                            null,
                            null,
                            "I LOVE HER LACK OF ENERGY! GO GIRL, GIVE US NOTHING!",
                            DateTime.Now.AddMinutes(-8),
                            1,
                            false
                        ),
                        new ReplyModel(
                            2,
                            new UserModel
                            {
                                Email = "jane.smith@example.com",
                                Username = "stelmakh_yurii",
                                Password = "password789",
                                Status = "active",
                                FieldOfStudy = "Engineering",
                                Specialization = "Embedded Systems",
                                University = "MIT",
                                Term = 4,
                                Photo = "pack://application:,,,/Resources/Images/TempUserIcons/stelmakh_yurii.png",
                                Chats = new List<ChatModel>()
                            },
                            null,
                            null,
                            "stfu hoe",
                            DateTime.Now.AddMinutes(-5),
                            5,
                            false
                        )
                    },
                    2,
                    false
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

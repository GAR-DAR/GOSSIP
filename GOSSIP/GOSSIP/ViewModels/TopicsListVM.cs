using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Список постів. Поки сиро і плачевно
    public class TopicsListVM : ObservableObject
    {
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<TopicVM> Topics { get; set; }

        private MainVM _mainVM;
        private JsonStorage _storage = new("topic_data.json");

        private TopicVM _selectedTopic;
        public TopicVM SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
            }
        }

        public ICommand DoubleClickCommand { get; }
        public ICommand LoadMoreCommand { get; }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            Topics = new(_storage.LoadTopics().Select(x => new TopicVM(x)));
            foreach(TopicVM topicVM in Topics)
            {
                topicVM.ProfileSelectedEvent += ProfileClickHandler;
            }

            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));
            LoadMoreCommand = new RelayCommand(LoadMoreMethod);

            
        }

        private void OnItemDoubleClickedMethod(TopicVM topic)
        {
            if (topic != null)
            {
                _mainVM.OpenTopic(topic);
            }
        }

        public void UpdateInfo()
        {
            JsonStorage jsonStorage = new("topic_data.json");
            Topics = new(_storage.LoadTopics().Select(x => new TopicVM(x)));
            foreach (var topic in Topics)
            {
                if (!Topics.Any(t => t.Topic.ID == topic.Topic.ID))
                {
                    Topics.Add(topic);
                }
                topic.ProfileSelectedEvent += ProfileClickHandler;
            }
        }

        private void LoadMoreMethod(object obj)
        {
            var loadedTopics = _storage.LoadTopics().Select(x => new TopicVM(x)).ToList();

            foreach (var topic in loadedTopics)
            {
                if (!Topics.Any(t => t.Topic.ID == topic.Topic.ID))
                {
                    topic.ProfileSelectedEvent += ProfileClickHandler;
                    Topics.Add(topic);
                }
            }
        }

        private void ProfileClickHandler(UserModel user)
        {
            _mainVM.OpenProfile(user);
        }
    }
}

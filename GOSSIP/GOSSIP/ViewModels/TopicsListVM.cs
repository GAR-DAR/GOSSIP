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
        public ObservableCollection<TopicModel> Topics { get; set; }

        private MainVM _mainVM;
        private JsonStorage _storage = new("topic_data.json");

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
        public ICommand LoadMoreCommand { get; }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            Topics = new(_storage.LoadTopics());
            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));
            LoadMoreCommand = new RelayCommand(LoadMoreMethod);
        }

        private void OnItemDoubleClickedMethod(TopicModel post)
        {
            if (post != null)
            {
                _mainVM.SelectedVM = new OpenedTopicVM(post, _mainVM);
            }
        }

        private void LoadMoreMethod(object obj)
        {
            var loadedTopics = _storage.LoadTopics();
            Topics.Clear();
            foreach (var topic in loadedTopics)
            {
                if (!Topics.Contains(topic)) 
                {
                    Topics.Add(topic);
                }
            }
        }
    }
}

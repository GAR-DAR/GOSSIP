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
    public class AuthUserProfileInfoVM : ObservableObject
    {
        private MainVM _mainVM;

        public UserModel User { get; set; }
        public string Header => "Profile";

        public ICommand DoubleClickCommand { get; }

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

        public ObservableCollection<TopicVM> Topics { get; set; } 

        public AuthUserProfileInfoVM(UserModel user, MainVM mainVM)
        {
            _mainVM = mainVM;
            User = user;
            TopicService topicService = new("topic_data.json");
            Topics = new(topicService.GetTopicsByID(User.ID).Select(x => new TopicVM(x)));
            DoubleClickCommand = new RelayCommand(obj => OnItemDoubleClickedMethod(SelectedTopic));
        }

        private void OnItemDoubleClickedMethod(TopicVM topicVM)
        {
            if (topicVM != null)
            {
                _mainVM.OpenTopic(topicVM);
            }
        }
    }
}

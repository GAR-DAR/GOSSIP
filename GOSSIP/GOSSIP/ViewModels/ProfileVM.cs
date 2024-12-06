using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GOSSIP.ViewModels
{
    public class ProfileVM : ObservableObject
    {
        public UserModel User { get; set; }
        private MainVM _mainVM;

        public ICommand BackCommand { get; set; }
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

        public ProfileVM(MainVM mainVM, UserModel user)
        {
            _mainVM = mainVM;
            User = user;
            BackCommand = new RelayCommand(BackMethod);

            TopicService topicService = new("topic_data.json");
            Topics = new(topicService.GetTopicsByID(User.ID).Select(x => new TopicVM(x)));

            DoubleClickCommand = new RelayCommand(obj => OnItemDoubleClickedMethod(SelectedTopic));
        }

        private void OnItemDoubleClickedMethod(TopicVM topicVM)
        {
            if (topicVM != null)
            {
                _mainVM.SelectedVM = new OpenedTopicVM(topicVM, _mainVM, this);
            }
        }

        private void BackMethod(object obj)
        {
            _mainVM.SwitchToPreviousVM();
        }

        public ObservableCollection<TopicVM> Topics { get; set; }


    }
}

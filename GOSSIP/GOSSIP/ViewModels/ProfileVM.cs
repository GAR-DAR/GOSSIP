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
    public class ProfileVM : ObservableObject
    {
        private UserVM _user;
        public UserVM User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        private MainVM _mainVM;
        public ObservableCollection<TopicVM> Topics { get; set; }

        private string _userInfo;
        public string UserInfo
        {
            get => _userInfo;
            set
            {
                _userInfo = value;
                OnPropertyChanged(nameof(UserInfo));
            }
        }

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

        public ICommand BackCommand { get; }
        public ICommand DoubleClickCommand { get; }

        public ProfileVM(MainVM mainVM, UserVM user)
        {
            _mainVM = mainVM;
            User = user;
            UpdateUserInfo();

            TopicService topicService = new("topic_data.json");
            Topics = new(topicService.GetTopicsByID(User.UserModel.ID).Select(x => new TopicVM(x)));

            BackCommand = new RelayCommand(BackMethod);
            DoubleClickCommand = new RelayCommand(obj => OnItemDoubleClickedMethod(SelectedTopic));
        }

        private void UpdateUserInfo()
        {
            UserInfo = $"Status: {User.Status}\nField of study: {User.FieldOfStudy}";

            if (User.Status == "Student" || User.Status == "Faculty")
            {
                UserInfo += $"\nSpecialization: {User.Specialization}\nUniversity: {User.University}\nDegree: {User.Degree}\nTerm: {User.Term}";
            }

            UserInfo += $"\nRole: {User.Role}";
        }

        private void BackMethod(object obj)
        {
            _mainVM.SwitchToPreviousVM();
        }

        private void OnItemDoubleClickedMethod(TopicVM topicVM)
        {
            if (topicVM != null)
            {
                _mainVM.SelectedVM = new OpenedTopicVM(topicVM, _mainVM);
            }
        }
    }
}

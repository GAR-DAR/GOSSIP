using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GOSSIP.ViewModels
{
    public class AuthUserProfileInfoVM : ObservableObject
    {
        private MainVM _mainVM;

        private UserVM _user;
        public UserVM User
        {
            get => _user;
            set
            {
                _user = value;
                UpdateUserInfo();
                OnPropertyChanged(nameof(User));
            }
        }

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

        private void UpdateUserInfo()
        {
            UserInfo = string.Empty;
            UserInfo = $"Status: {User.Status}\nField of study: {User.FieldOfStudy}";
            if(User.Status == "Student" || User.Status == "Faculty")
            {
                UserInfo += $"\nSpecialization: {User.Specialization}\nUniversity: {User.University}\nDegree: {User.Degree}\nTerm: {User.Term}";
            }
            UserInfo += $"\nRole: {User.Role}";
        }

        public ObservableCollection<TopicVM> Topics { get; set; } 

        public AuthUserProfileInfoVM(UserVM user, MainVM mainVM)
        {
            _mainVM = mainVM;
            User = user;

            UpdateUserInfo();

           
            DoubleClickCommand = new RelayCommand(obj => OnItemDoubleClickedMethod(SelectedTopic));

            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Status)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.FieldOfStudy)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Specialization)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.University)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Degree)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Term)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Role)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Username)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Password)
            );
            PropertyChangedEventManager.AddHandler(
                User,
                (s, e) => UpdateUserInfo(),
                nameof(User.Email)
            );
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

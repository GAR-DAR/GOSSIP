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
    public class OpenedTopicVM : ObservableObject
    {
        public TopicModel Topic { get; set; }

        private bool _canUpVote = true;
        private bool _canDownVote = true;
        private MainVM _mainVM;

        public ICommand BackCommand { get; set; }
        public ICommand UpVoteTopicCommand { get; set; }
        public ICommand DownVoteTopicCommand { get; set; }
        public ICommand AddReplyCommand { get; set; }
        

        public UserModel Author
        {
            get => Topic.Author;
            set
            {
                Topic.Author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public int Rating
        {
            get => Topic.Rating;
            set
            {
                Topic.Rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public string Content
        {
            get => Topic.Content;
            set
            {
                Topic.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string Title
        {
            get => Topic.Title;
            set
            {
                Topic.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public DateTime CreatedAt
        {
            get => Topic.CreatedAt;
            set
            {
                Topic.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public int RepliesCount
        {
            get => Topic.RepliesCount;
            set
            {
                Topic.RepliesCount = value;
                OnPropertyChanged(nameof(RepliesCount));
            }
        }

        public ObservableCollection<ReplyModel> Replies { get; set; }

        private string _enteredReplyText;
        public string EnteredReplyText
        {
            get => _enteredReplyText;
            set
            {
                _enteredReplyText = value;
                OnPropertyChanged(nameof(EnteredReplyText));
            }
        }

        public OpenedTopicVM(TopicModel topic, MainVM mainVM)
        {
            _mainVM = mainVM;
            Topic = topic;
            Replies = new(Topic.Replies);

            BackCommand = new RelayCommand(_mainVM.ShowPostsListMethod);
            UpVoteTopicCommand = new RelayCommand(UpVoteMethod);
            DownVoteTopicCommand = new RelayCommand(DownVoteMethod);
            AddReplyCommand = new RelayCommand(AddReplyMethod);
        }

        private void UpVoteMethod(object obj)
        {
            if (_mainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }

            if (_canUpVote)
            {
                Rating++;
                _canUpVote = false;
                if (_canDownVote == false)
                {
                    Rating++;
                    _canDownVote = true;
                }
            }
            else
            {
                Rating--;
                _canUpVote = true;
            }
        }

        private void DownVoteMethod(object obj)
        {
            if (_mainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }

            if (_canDownVote)
            {
                Rating--;
                _canDownVote = false;
                if (_canUpVote == false)
                {
                    Rating--;
                    _canUpVote = true;
                }
            }
            else
            {
                Rating++;
                _canDownVote = true;
            }
        }

        private void AddReplyMethod(object obj)
        {
            if (_mainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }


            if (!string.IsNullOrEmpty(EnteredReplyText))
            {
                Replies.Add(new ReplyModel(1, _mainVM.AuthorizedUser, Topic, null, EnteredReplyText, DateTime.Now, 0, false));
                RepliesCount++;
                Topic.Replies.Add(new ReplyModel(1, _mainVM.AuthorizedUser, Topic, null, EnteredReplyText, DateTime.Now, 0, false));
                EnteredReplyText = "";
            }
        }
    }
}

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

        public ICommand BackCommand { get; set; }
        public ICommand UpVoteTopicCommand { get; set; }
        public ICommand DownVoteTopicCommand { get; set; }

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

        public int RepliesCount => Topic.Replies.Count;

        public ObservableCollection<ReplyModel> Replies { get; set; }

        public OpenedTopicVM(TopicModel topic, MainVM mainVM)
        {
            Topic = topic;
            Replies = new(Topic.Replies);

            BackCommand = new RelayCommand(mainVM.ShowPostsListMethod);
            UpVoteTopicCommand = new RelayCommand(UpVoteMethod);
            DownVoteTopicCommand = new RelayCommand(DownVoteMethod);
        }

        private void UpVoteMethod(object obj)
        {
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
    }
}

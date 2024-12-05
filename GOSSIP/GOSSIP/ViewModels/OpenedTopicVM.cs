using GOSSIP.JsonHandlers;
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
        private JsonStorage _jsonStorage = new("topic_data.json");

        public ICommand BackCommand { get; set; }
        public ICommand UpVoteTopicCommand { get; set; }
        public ICommand DownVoteTopicCommand { get; set; }
        public ICommand AddReplyCommand { get; set; }
        public ICommand UpVoteReplyCommand { get; set; }
        public ICommand DownVoteReplyCommand { get; set; }
        public ICommand ReplyToReplyCommand { get; set; }

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

        public uint RepliesCount
        {
            get => Topic.RepliesCount;
            set
            {
                Topic.RepliesCount = value;
                OnPropertyChanged(nameof(RepliesCount));
            }
        }

        public ObservableCollection<ReplyVM> Replies { get; set; }

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
            Replies = new(Topic.Replies.Select(x => new ReplyVM(x)));

            BackCommand = new RelayCommand(_mainVM.ShowPostsListMethod);
            UpVoteTopicCommand = new RelayCommand(UpVoteMethod);
            DownVoteTopicCommand = new RelayCommand(DownVoteMethod);
            AddReplyCommand = new RelayCommand(AddReplyMethod);
            UpVoteReplyCommand = new RelayCommand(UpVoteReplyMethod);
            DownVoteReplyCommand = new RelayCommand(DownVoteReplyMethod);
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
            
            _jsonStorage.SaveTopic(Topic);
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

            _jsonStorage.SaveTopic(Topic);
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
                Replies.Add(new ReplyVM(new ReplyModel(1, _mainVM.AuthorizedUser, Topic, null, EnteredReplyText, DateTime.Now, 0, false)));
                RepliesCount++;
                Topic.Replies.Add(new ReplyModel(1, _mainVM.AuthorizedUser, Topic, null, EnteredReplyText, DateTime.Now, 0, false));
                EnteredReplyText = "";
            }

            _jsonStorage.SaveTopic(Topic);
        }

        private void UpVoteReplyMethod(object obj)
        {
            if (_mainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ReplyVM reply)
            {
                if (reply.CanUpVote)
                {
                    reply.Rating++;
                    reply.CanUpVote = false;
                    if (reply.CanDownVote == false)
                    {
                        reply.Rating++;
                        reply.CanDownVote = true;
                    }
                }
                else
                {
                    reply.Rating--;
                    reply.CanUpVote = true;
                }
            }

            _jsonStorage.SaveTopic(Topic);
        }

        private void DownVoteReplyMethod(object obj)
        {
            if (_mainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ReplyVM reply)
            {
                if (reply.CanDownVote)
                {
                    reply.Rating--;
                    reply.CanDownVote = false;
                    if (reply.CanUpVote == false)
                    {
                        reply.Rating--;
                        reply.CanUpVote = true;
                    }
                }
                else
                {
                    reply.Rating++;
                    reply.CanDownVote = true;
                }
            }

            _jsonStorage.SaveTopic(Topic);
        }
    }
}

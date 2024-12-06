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
        public TopicVM TopicVM { get; set; }

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
        public ICommand UpVoteReplyOnReplyCommand { get; set; }
        public ICommand DownVoteReplyOnReplyCommand { get; set; }

        public UserModel Author
        {
            get => TopicVM.Topic.Author;
            set
            {
                TopicVM.Topic.Author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public int? Rating
        {
            get => TopicVM.Rating;
            set
            {
                TopicVM.Rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public string Content
        {
            get => TopicVM.Content;
            set
            {
                TopicVM.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string Title
        {
            get => TopicVM.Title;
            set
            {
                TopicVM.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public DateTime CreatedAt
        {
            get => TopicVM.CreatedAt;
            set
            {
                TopicVM.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public uint RepliesCount
        {
            get => TopicVM.RepliesCount;
            set
            {
                TopicVM.RepliesCount = value;
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

        public OpenedTopicVM(TopicVM topic, MainVM mainVM, ObservableObject previousVM)
        {
            _mainVM = mainVM;
            TopicVM = topic;
            Replies = new(TopicVM.Topic.Replies.Select(x => new ReplyVM(x)));

            BackCommand = new RelayCommand((obj) => _mainVM.SelectedVM = previousVM);
            UpVoteTopicCommand = new RelayCommand(UpVoteMethod);
            DownVoteTopicCommand = new RelayCommand(DownVoteMethod);
            AddReplyCommand = new RelayCommand(AddReplyMethod);
            UpVoteReplyCommand = new RelayCommand(UpVoteReplyMethod);
            DownVoteReplyCommand = new RelayCommand(DownVoteReplyMethod);
            UpVoteReplyOnReplyCommand = new RelayCommand(UpVoteReplyOnReplyMethod);
            DownVoteReplyOnReplyCommand = new RelayCommand(DownVoteReplyOnReplyMethod);
        }

        private void UpVoteMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
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
            
            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void DownVoteMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
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

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void AddReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }


            if (!string.IsNullOrEmpty(EnteredReplyText))
            {
                Replies.Add(new ReplyVM(new ParentReplyModel(1, MainVM.AuthorizedUser, TopicVM.Topic, EnteredReplyText, DateTime.Now, 0, false, [])));
                RepliesCount++;
                TopicVM.Topic.Replies.Add(new ParentReplyModel(1, MainVM.AuthorizedUser, TopicVM.Topic, EnteredReplyText, DateTime.Now, 0, false, []));
                EnteredReplyText = "";
            }

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void UpVoteReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
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

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void UpVoteReplyOnReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ChildReplyVM reply)
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

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void DownVoteReplyOnReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ChildReplyVM reply)
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

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void DownVoteReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUser == null)
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

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }
    }
}

using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GOSSIP.ViewModels
{
    public class ReplyVM : ObservableObject
    {
        private readonly ParentReplyModel _replyModel;

        public bool CanUpVote { get; set; } = true;
        public bool CanDownVote { get; set; } = true;

        public uint ID
        {
            get => _replyModel.ID;
            set
            {
                if (_replyModel.ID != value)
                {
                    _replyModel.ID = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        public ICommand ShowRepliesToReplyCommand { get; set; }
        public ICommand ShowReplyQuery { get; set; }
        public ICommand SendReplyToReply { get; set; }
        public ICommand SendReplyToReplyToReplyCommand { get; set; }

        public UserModel User
        {
            get => _replyModel.User;
            set
            {
                if (_replyModel.User != value)
                {
                    _replyModel.User = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Content
        {
            get => _replyModel.Content;
            set
            {
                if (_replyModel.Content != value)
                {
                    _replyModel.Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _replyModel.CreatedAt;
            set
            {
                _replyModel.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public int Rating
        {
            get => _replyModel.Rating;
            set
            {
                if (_replyModel.Rating != value)
                {
                    _replyModel.Rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        private int _countOfReplies;
        public int CountOfReplies
        {
            get => _countOfReplies;
            set
            {
                _countOfReplies = value;
                OnPropertyChanged(nameof(CountOfReplies));
            }
        }

        private bool _isRepliesListNotEmpty;
        public bool IsRepliesListNotEmpty
        {
            get => _isRepliesListNotEmpty;
            set
            {
                _isRepliesListNotEmpty = value;
                OnPropertyChanged(nameof(IsRepliesListNotEmpty));
            }
        }

        private bool _isShowRepliesPressed;
        public bool IsShowRepliesPressed
        {
            get => _isShowRepliesPressed;
            set
            {
                _isShowRepliesPressed = value;
                OnPropertyChanged(nameof(IsShowRepliesPressed));
            }
        }

        private bool _isReplyButtonPressed;
        public bool IsReplyButtonPressed
        {
            get => _isReplyButtonPressed;
            set
            {
                _isReplyButtonPressed = value;
                OnPropertyChanged(nameof(IsReplyButtonPressed));
            }
        }

        private string _replyToReplyContent;
        public string ReplyToReplyContent
        {
            get => _replyToReplyContent;
            set
            {
                _replyToReplyContent = value;
                OnPropertyChanged(nameof(ReplyToReplyContent));
            }
        }

        public ObservableCollection<ChildReplyVM> Replies { get; set; }

        public ReplyVM(ParentReplyModel replyModel)
        {
            _replyModel = replyModel;
            Replies = new(replyModel.Replies.Select(x => new ChildReplyVM(x, _replyModel, this)));
            CountOfReplies = Replies.Count;
            
            ShowRepliesToReplyCommand = new RelayCommand((obj) => IsShowRepliesPressed = !IsShowRepliesPressed);
            ShowReplyQuery = new RelayCommand((obj) => IsReplyButtonPressed = !IsReplyButtonPressed);
            SendReplyToReply = new RelayCommand(SendReplyToReplyMethod);
            
            IsRepliesListNotEmpty = Replies.Count > 0;
        }

        private void SendReplyToReplyMethod(object obj)
        {
            if (string.IsNullOrEmpty(ReplyToReplyContent))
            {
                return;
            }

            ChildReplyModel childReply = new()
            {
                Content = ReplyToReplyContent,
                CreatedAt = DateTime.Now,
                ID = 0,
                Rating = 0,
                User = MainVM.AuthorizedUserVM.UserModel,
                ReplyTo = _replyModel.User
            };

            Replies.Add(new ChildReplyVM(childReply, _replyModel, this));
            _replyModel.Replies.Add(childReply);
            CountOfReplies++;
            IsReplyButtonPressed = true;

            IsRepliesListNotEmpty = Replies.Count > 0;

            JsonStorage jsonStorage = new("topic_data.json");
            jsonStorage.SaveTopic(_replyModel.Topic);

            ReplyToReplyContent = "";
        }
    }
}

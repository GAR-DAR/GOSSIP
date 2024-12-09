using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class ParentReplyVM : ObservableObject
    {
        private readonly ParentReplyModel _replyModel;

        private bool _canUpVote = true;
        public bool CanUpVote
        {
            get => _canUpVote;
            set
            {
                _canUpVote = value;
                OnPropertyChanged(nameof(CanUpVote));
            }
        }

        private bool _canDownVote = true;
        public bool CanDownVote
        {
            get => _canDownVote;
            set
            {
                _canDownVote = value;
                OnPropertyChanged(nameof(CanDownVote));
            }
        }

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

        public ObservableCollection<ChildReplyVM> Replies { get; }
        public ICommand ShowRepliesToReplyCommand { get; }
        public ICommand ShowReplyQuery { get; }
        public ICommand SendReplyToReply { get; }
        public ICommand CommentAuthorProfileClickCommand { get; set; }
        public event Action<UserVM> ProfileClickEvent;
        public event Action<object> UserIsNotAuthorized;

        public ParentReplyVM(ParentReplyModel replyModel)
        {
            _replyModel = replyModel;
            Replies = new(replyModel.Replies.Select(x => new ChildReplyVM(x, _replyModel, this)));
            CountOfReplies = Replies.Count;

            ShowRepliesToReplyCommand = new RelayCommand(obj => IsShowRepliesPressed = !IsShowRepliesPressed);
            ShowReplyQuery = new RelayCommand(obj => IsReplyButtonPressed = !IsReplyButtonPressed);
            SendReplyToReply = new RelayCommand(SendReplyToReplyMethod);
            CommentAuthorProfileClickCommand = new RelayCommand(obj => ProfileClickEvent?.Invoke(new(replyModel.User)));

            foreach(ChildReplyVM childReplyVM in Replies)
            {
                childReplyVM.UserIsNotAuthorized += UserIsNotAuthorizedHandler;
                childReplyVM.ProfileClickEvent += ProfileClickHandler;
            }

            IsRepliesListNotEmpty = Replies.Count > 0;
        }

        private void ProfileClickHandler(UserVM user)
        {
            ProfileClickEvent?.Invoke(user);
        }

        private void UserIsNotAuthorizedHandler()
        {
            UserIsNotAuthorized?.Invoke(null);
        }

        private void SendReplyToReplyMethod(object obj)
        {
            if(MainVM.AuthorizedUserVM == null)
            {
                UserIsNotAuthorizedHandler();
                return;
            }

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

            ChildReplyVM childReplyVM = new(childReply, _replyModel, this);
            childReplyVM.ProfileClickEvent += ProfileClickHandler;
            childReplyVM.UserIsNotAuthorized += UserIsNotAuthorizedHandler;

            Replies.Add(childReplyVM);
            _replyModel.Replies.Add(childReply);
            CountOfReplies++;
            IsReplyButtonPressed = true;

            IsRepliesListNotEmpty = Replies.Count > 0;

            JsonStorage jsonStorage = new("topic_data.json");
            jsonStorage.SaveTopic(_replyModel.Topic);

            ReplyToReplyContent = string.Empty;
        }
    }
}

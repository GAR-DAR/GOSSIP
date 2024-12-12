
using GOSSIP.Net;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class ParentReplyVM : ObservableObject
    {
        public ParentReplyModel ReplyModelPR { get; }

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
            get => ReplyModelPR.ID;
            set
            {
                if (ReplyModelPR.ID != value)
                {
                    ReplyModelPR.ID = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        public UserModel User
        {
            get => ReplyModelPR.User;
            set
            {
                if (ReplyModelPR.User != value)
                {
                    ReplyModelPR.User = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Content
        {
            get => ReplyModelPR.Content;
            set
            {
                if (ReplyModelPR.Content != value)
                {
                    ReplyModelPR.Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => ReplyModelPR.CreatedAt;
            set
            {
                ReplyModelPR.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public int Rating
        {
            get => ReplyModelPR.Rating;
            set
            {
                if (ReplyModelPR.Rating != value)
                {
                    ReplyModelPR.Rating = value;
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
            ReplyModelPR = replyModel;
            Replies = new(replyModel.Replies.Select(x => new ChildReplyVM(x, ReplyModelPR, this)));
            CountOfReplies = Replies.Count;

            ShowRepliesToReplyCommand = new RelayCommand(obj => IsShowRepliesPressed = !IsShowRepliesPressed);
            ShowReplyQuery = new RelayCommand(obj => IsReplyButtonPressed = !IsReplyButtonPressed);
            SendReplyToReply = new RelayCommand(SendReplyToReplyMethod);
            CommentAuthorProfileClickCommand = new RelayCommand(obj => ProfileClickEvent?.Invoke(new(replyModel.User)));

            Globals.server.getReplyOnReply += SendReplyToReplyMethod;

            foreach (ChildReplyVM childReplyVM in Replies)
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
           

            if (MainVM.AuthorizedUserVM == null)
            {
                UserIsNotAuthorizedHandler();
                return;
            }


            if (string.IsNullOrEmpty(ReplyToReplyContent))
            {
                return;
            }

            obj = obj as ChildReplyModel;

            ChildReplyModel childReply = new()
            {
                Content = ReplyToReplyContent,
                CreatedAt = DateTime.Now,
                ID = 0,
                Rating = 0,
                User = MainVM.AuthorizedUserVM.UserModel,
                ReplyTo = ReplyModelPR.User,
                Topic = ReplyModelPR.Topic
            };

            ChildReplyVM childReplyVM = new(childReply, ReplyModelPR, this);
            childReplyVM.ProfileClickEvent += ProfileClickHandler;
            childReplyVM.UserIsNotAuthorized += UserIsNotAuthorizedHandler;

            Globals.server.SendPacket(SignalsEnum.CreateReplyToReply, new ChildReplyModelID(childReply));

            Replies.Add(childReplyVM);
            ReplyModelPR.Replies.Add(childReply);
            CountOfReplies++;
            IsReplyButtonPressed = true;

            IsRepliesListNotEmpty = Replies.Count > 0;

            ReplyToReplyContent = string.Empty;
        }
    }
}

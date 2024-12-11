﻿using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class ChildReplyVM : ObservableObject
    {
        private readonly ChildReplyModel _replyModel;
        private readonly ParentReplyModel _parentReplyModel;
        private readonly ParentReplyVM _replyVM;


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

        public ICommand ShowReplyQueryCommand { get; set; }
        public ICommand SendReplyCommand { get; set; }
        public ICommand CommentAuthorProfileClickCommand { get; set; }
        public event Action<UserVM> ProfileClickEvent;
        public event Action UserIsNotAuthorized;

        private string _replyQuery;
        public string ReplyQuery
        {
            get => _replyQuery;
            set
            {
                _replyQuery = value;
                OnPropertyChanged(nameof(ReplyQuery));
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

        public ChildReplyVM(ChildReplyModel replyModel, ParentReplyModel parentReplyModel, ParentReplyVM replyVM)
        {
            _replyModel = replyModel;
            _parentReplyModel = parentReplyModel;
            _replyVM = replyVM;
            ShowReplyQueryCommand = new RelayCommand(obj => { IsReplyButtonPressed = !IsReplyButtonPressed; });
            SendReplyCommand = new RelayCommand(SendReplyMethod);
            CommentAuthorProfileClickCommand = new RelayCommand(obj => ProfileClickEvent?.Invoke(new(replyModel.User)));
        }

        private void SendReplyMethod(object obj)
        {
            if(MainVM.AuthorizedUserVM == null)
            {
                UserIsNotAuthorized?.Invoke();
                return;
            }

            if (string.IsNullOrEmpty(ReplyQuery))
                return;

            ChildReplyModel childReplyModel = new(
                ID = 0,
                MainVM.AuthorizedUserVM.UserModel,
                null,
                ReplyQuery,
                DateTime.Now,
                0,
                false,
                _replyModel.User);

            _parentReplyModel.Replies.Add(childReplyModel);
            _replyVM.Replies.Add(new(childReplyModel, _parentReplyModel, _replyVM));

        }
    }
}
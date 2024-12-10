using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using GOSSIP.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
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
        public ICommand TopicAuthorProfileClickCommand { get; set; }

        public UserModel Author
        {
            get => TopicVM.Topic.Author;
            set
            {
                TopicVM.Topic.Author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public int Rating
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

        public ObservableCollection<ParentReplyVM> Replies { get; set; }

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

            Replies = new(TopicVM.Topic.Replies.Select(x => new ParentReplyVM(x)));

            BackCommand = new RelayCommand((obj) => _mainVM.SelectedVM = previousVM);
            UpVoteTopicCommand = new RelayCommand(UpVoteMethod);
            DownVoteTopicCommand = new RelayCommand(DownVoteMethod);
            AddReplyCommand = new RelayCommand(AddReplyMethod);
            UpVoteReplyCommand = new RelayCommand(UpVoteReplyMethod);
            DownVoteReplyCommand = new RelayCommand(DownVoteReplyMethod);
            UpVoteReplyOnReplyCommand = new RelayCommand(UpVoteReplyOnReplyMethod);
            DownVoteReplyOnReplyCommand = new RelayCommand(DownVoteReplyOnReplyMethod);
            TopicAuthorProfileClickCommand = new RelayCommand(obj => _mainVM.OpenProfile(new(topic.Topic.Author)));
            
            foreach(ParentReplyVM reply in Replies)
            {
                reply.UserIsNotAuthorized += _mainVM.ShowLogInMethod;
                reply.ProfileClickEvent += _mainVM.OpenProfile;
            }
        }

        private void UpVoteMethod(object obj)
        {
            if (MainVM.AuthorizedUserVM == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }

            if (CanUpVote)
            {
                Rating++;
                CanUpVote = false;
                if (CanDownVote == false)
                {
                    Rating++;
                    CanDownVote = true;
                }
            }
            else
            {
                Rating--;
                CanUpVote = true;
            }
            
            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void DownVoteMethod(object obj)
        {
            if (MainVM.AuthorizedUserVM == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }

            if (CanDownVote)
            {
                Rating--;
                CanDownVote = false;
                if (CanUpVote == false)
                {
                    Rating--;
                    CanUpVote = true;
                }
            }
            else
            {
                Rating++;
                CanDownVote = true;
            }

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void AddReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUserVM == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }

            if (!string.IsNullOrEmpty(EnteredReplyText))
            {
				ParentReplyVM newReply = new ParentReplyVM(new ParentReplyModel(1, MainVM.AuthorizedUserVM.UserModel, TopicVM.Topic, EnteredReplyText, DateTime.Now, 0, false, []));
                newReply.ProfileClickEvent += _mainVM.OpenProfile;
                newReply.UserIsNotAuthorized += _mainVM.ShowLogInMethod;

                
                
                Globals.server.SendPacket(SignalsEnum.CreateReply, newReply);

                Replies.Add(newReply);
                RepliesCount++;
				
                TopicVM.Topic.Replies.Add(newReply.ReplyModelPR);

                EnteredReplyText = "";
            }

            _jsonStorage.SaveTopic(TopicVM.Topic);
        }

        private void UpVoteReplyMethod(object obj)
        {
            if (MainVM.AuthorizedUserVM == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ParentReplyVM reply)
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
            if (MainVM.AuthorizedUserVM == null)
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
            if (MainVM.AuthorizedUserVM == null)
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
            if (MainVM.AuthorizedUserVM == null)
            {
                _mainVM.ShowLogInMethod(null);
                return;
            }
            if (obj is ParentReplyVM reply)
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

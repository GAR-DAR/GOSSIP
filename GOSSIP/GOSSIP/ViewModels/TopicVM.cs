using GOSSIP.Models.IDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class TopicVM : ObservableObject
    {
        public TopicModel Topic { get; set; }
        private TopicsListVM _topicListVM;

        public ICommand PhotoClickCommand { get; set; }

        public event Action<UserVM> ProfileSelectedEvent;

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
        
        public int Rating
        {
            get => Topic.Rating;
            set
            {
                Topic.Rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public List<string> Tags
        {
            get => Topic.Tags;
            set
            {
                Topic.Tags = value;
                OnPropertyChanged(nameof(Tags));
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

        public TopicVM(TopicModel topic)
        {
            Topic = topic;
            PhotoClickCommand = new RelayCommand(PhotoClickMethod);
        }

        private void PhotoClickMethod(object obj)
        {
            ProfileSelectedEvent?.Invoke(new(Topic.AuthorID));
        }

    }
}

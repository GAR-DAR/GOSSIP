using GOSSIP.Net;
using GOSSIP.Net.IO;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Список постів. Поки сиро і плачевно
    public class TopicsListVM : ObservableObject
    {
        private ObservableCollection<TopicVM> _topics = [];
        public ObservableCollection<TopicVM> Topics
        {
            get => _topics;
            set
            {
                _topics = value;
                OnPropertyChanged(nameof(Topics));
            }
        }

        private ObservableCollection<TopicVM> _filteredCollection;
        public ObservableCollection<TopicVM> FilteredTopics
        {
            get => _filteredCollection;
            set
            {
                _filteredCollection = value;
                OnPropertyChanged(nameof(FilteredTopics));
            }
        }

        private MainVM _mainVM;
       
        private TopicVM _selectedTopic;
        public TopicVM SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
            }
        }

        public ICommand DoubleClickCommand { get; }
        public ICommand LoadMoreCommand { get; }


        public async Task LoadTopicsAsync()
        {
            Globals.server.getTopicsEvent += (topics) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Topics = new ObservableCollection<TopicVM>(topics.Select(x => new TopicVM(x)));
                    foreach (var topic in Topics)
                    {
                        topic.ProfileSelectedEvent += _mainVM.OpenProfile;
                        foreach (var reply in topic.Topic.Replies)
                        {
                            reply.Topic = topic.Topic;
                        }
                    }
                    FilteredTopics = Topics;
                });
            };
        }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));
            LoadMoreCommand = new RelayCommand(LoadMoreMethod);
            Task.Run(async () => await LoadTopicsAsync());
        }

        public void SearchMethod(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
                FilteredTopics = Topics;
            else
                FilteredTopics = new ObservableCollection<TopicVM>(Topics.Where(topic =>
                    topic.Content.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    topic.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    topic.Tags.Any(tag => tag.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))));
        }

        private void LoadMoreMethod(object obj)
        {
           Globals.server.SendPacket(SignalsEnum.Refresh);
        }

        private void ProfileClickHandler(UserVM user)
        {
            ProfileVM profileVM = new(_mainVM, user);
            _mainVM.SelectedVM = profileVM;
            _mainVM.StackOfVMs.Add(profileVM);
        }

        private void OnItemDoubleClickedMethod(TopicVM topic)
        {
            if (topic != null)
            {
                Globals.server.SendPacket(SignalsEnum.GetReplies, topic.Topic.ID);
            }
        }

        public void UpdateInfo()
        {
            foreach (var topic in Topics)
            {
                if (!Topics.Any(t => t.Topic.ID == topic.Topic.ID))
                {
                    Topics.Add(topic);
                }
                topic.ProfileSelectedEvent += ProfileClickHandler;
            }
        }
    }
}

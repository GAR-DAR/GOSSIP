

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
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<TopicVM> _topics = [];
        public ObservableCollection<TopicVM> Topics
        {
            get => _topics;
            set
            {
                _topics = value;
                OnPropertyChanged(nameof(Topics));
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
                        foreach (var reply in topic.Topic.Replies)
                        {
                            reply.Topic = topic.Topic;
                        }
                    }
                });
            };
        }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            //Topics = new(_storage.LoadTopics().Select(x => new TopicVM(x)));
            //foreach(TopicVM topicVM in Topics)
            //{
            //    topicVM.ProfileSelectedEvent += ProfileClickHandler;
            //}


            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));
            LoadMoreCommand = new RelayCommand(LoadMoreMethod);
            Task.Run(async () => await LoadTopicsAsync());
        }

        private void LoadMoreMethod(object obj)
        {
           
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
                //_mainVM.OpenTopic(topic);
                //Globals.server.SendPacket(SignalsEnum.GetReplies, topic.Topic.ID);
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

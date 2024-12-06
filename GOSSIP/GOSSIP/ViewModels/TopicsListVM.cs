using GOSSIP.Models;
using GOSSIP.Net;
using GOSSIP.Net.IO;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Список постів. Поки сиро і плачевно
    public class TopicsListVM : ObservableObject
    {
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<TopicModel> _topics;
        public ObservableCollection<TopicModel> Topics
        {
            get => _topics;
            set
            {
                _topics = value;
                OnPropertyChanged(nameof(Topics));
            }
        }

        private MainVM _mainVM;

        private TopicModel _selectedTopic;
        public TopicModel SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
            }
        }

        public ICommand DoubleClickCommand { get; }


        public async Task LoadTopicsAsync()
        {
            Globals.server.getTopicsEvent += (topics) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Topics = new ObservableCollection<TopicModel>(topics);
                    foreach (var topic in Topics)
                    {
                        foreach (var reply in topic.Replies)
                        {
                            reply.Topic = topic;
                        }
                    }
                });
            };
        }

        public TopicsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));

            Task.Run(async () => await LoadTopicsAsync());
        }

        //public TopicsListVM(MainVM mainVM)
        //{
        //    //Globals.server.getTopicsEvent += (topics) =>
        //    //{
        //    //    Application.Current.Dispatcher.Invoke(() =>
        //    //    {
                   
        //    //        Topics = new ObservableCollection<TopicModel>(topics);
        //    //        foreach (var topic in Topics)
        //    //        {
        //    //            foreach (var reply in topic.Replies)
        //    //            {
        //    //                reply.Topic = topic;
        //    //            }
        //    //        }
        //    //    });
        //    //};

        //    _mainVM = mainVM;

        //    Task.Run(async () => await InitializeTopics());

        //    DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedTopic));

        //    if (Topics != null)
        //    {
        //        var topic = Topics[0];
        //        foreach (var reply in topic.Replies)
        //        {
        //            reply.Topic = topic;
        //        }
        //    }
            
        //}

        private void OnItemDoubleClickedMethod(TopicModel post)
        {
            if (post != null)
            {
                _mainVM.SelectedVM = new OpenedTopicVM(post, _mainVM);
            }
        }
    }
}

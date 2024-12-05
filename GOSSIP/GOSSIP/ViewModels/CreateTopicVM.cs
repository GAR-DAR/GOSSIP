using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;

namespace GOSSIP.ViewModels
{
    public class CreateTopicVM : ObservableObject
    {
        private MainVM _mainVM;
        public ICommand BackCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand CreateTopicCommand { get; }
        public ICommand SubmitTagCommand { get; }

        private ObservableCollection<string> AllTags;

        public ObservableCollection<string> FilteredTags { get; set; } = [];

        private string _selectedTag;
        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                _selectedTag = value;
                OnPropertyChanged(nameof(SelectedTag));

                if (_selectedTag == null || SelectedTags.Contains(_selectedTag))
                {
                    SearchQuery = null;
                    return;
                }
                
                if (!string.IsNullOrEmpty(SelectedTag))
                {
                    SelectedTags.Add(_selectedTag);
                    AllTags.Remove(_selectedTag);
                    SelectedTag = null;
                }
                SearchQuery = null;
            }
        }

        public ObservableCollection<string> SelectedTags { get; set; } = [];

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (string.IsNullOrEmpty(_searchQuery))
                    IsEmpty = false;

                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                FilterTags();   
            }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        private void FilterTags()
        { 
            if (string.IsNullOrEmpty(SearchQuery))
            {
                FilteredTags.Clear();
                return;
            }

            var matches = AllTags
                .Where(tag => tag.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            FilteredTags.Clear(); // Завжди очищайте список перед оновленням
            foreach (var match in matches)
            {
                if (!FilteredTags.Contains(match)) // Перевірка дублювання
                {
                    FilteredTags.Add(match);
                }
            }

            if (FilteredTags.Count == 0)
                IsEmpty = false;
            else
                IsEmpty = true;
        }

        public CreateTopicVM(MainVM mainVM)
        {
            AllTags =
                [
                    "C#",
                    "C++",
                    "C",
                    "Java"
                ];

            _mainVM = mainVM;
            BackCommand = new RelayCommand(mainVM.ShowPostsListMethod);
            RemoveTagCommand = new RelayCommand(RemoveTagMethod);
            CreateTopicCommand = new RelayCommand(CreateTopicMethod);
            SubmitTagCommand = new RelayCommand(SubmitTagMethod);
        }

        private void CreateTopicMethod(object obj)
        {
            if(string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Content))
                return;

            TopicModel topic = new(
                    0,
                    _mainVM.AuthorizedUser,
                    Title,
                    Content,
                    DateTime.Now,
                    0,
                    SelectedTags.ToList(),
                    [],
                    0,
                    false
                );

            TopicService topicService = new("topic_data.json");
            topicService.AddTopic(topic);
            _mainVM.ShowPostsListMethod(null);
            
        }

        private void RemoveTagMethod(object tag)
        {
            SelectedTags.Remove((string)tag);
            AllTags.Add((string)tag);
        }

        private void SubmitTagMethod(object obj)
        {
            if (!SelectedTags.Contains(SearchQuery))
            {
                SelectedTags.Add(SearchQuery);
            }

            SearchQuery = null;
        }
    }
}

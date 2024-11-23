using GOSSIP.Models;
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
    public class PostsListVM : ObservableObject
    {
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<Post> Posts { get; set; }

        private MainVM _mainVM;

        private Post _selectedPost;
        public Post SelectedPost
        {
            get => _selectedPost;
            set
            {
                _selectedPost = value;
                OnPropertyChanged(nameof(SelectedPost));
            }
        }

        public ICommand DoubleClickCommand { get; }

        public PostsListVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            DoubleClickCommand = new RelayCommand((obj) => OnItemDoubleClickedMethod(SelectedPost));

            Posts =
            [
                new Post(
                    new UserModel(
                        "stelmakh.yurii@example.com",
                        "stelmakh_yurii",
                        "password123",
                        "active",
                        "Computer Science",
                        "Software Development",
                        "Lviv Polytechnic",
                        2,
                        "stelmakh_yurii.png",
                        new List<ChatModel>()
                    ),
                    DateTime.Now.AddMinutes(-10),
                    "What C# GUI framework for desktop apps is the best?",
                    "filler text, filler text, filler text, filler text",
                    new List<string> { "C#", "GUI" },
                    3,
                    2
                ),
                new Post(
                    new UserModel(
                        "oleksa.lviv@example.com",
                        "OleksaLviv",
                        "password456",
                        "online",
                        "Engineering",
                        "Hardware Design",
                        "Kyiv Polytechnic",
                        3,
                        "OleksaLviv.png",
                        new List<ChatModel>()
                    ),
                    DateTime.Now.AddDays(-20),
                    "Can yall share a quality resource about WinAPI?",
                    "...",
                    new List<string> { "WinAPI", "Windows" },
                    3,
                    2
                )
            ];
        }

        private void OnItemDoubleClickedMethod(Post post)
        {
            if (post != null)
            {
                _mainVM.SelectedVM = new OpenedPostVM(post);
            }
        }
    }
}

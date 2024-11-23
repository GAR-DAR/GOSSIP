using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class OpenedPostVM : ObservableObject
    {
        public Post Post { get; set; }

        public UserModel Author
        {
            get => Post.Author;
            set
            {
                Post.Author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public int Rating
        {
            get => Post.Rating;
            set
            {
                Post.Rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public string Content
        {
            get => Post.Content;
            set
            {
                Post.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string Title
        {
            get => Post.Title;
            set
            {
                Post.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }


        public OpenedPostVM(Post post)
        {
            Post = post;
        }
    }
}

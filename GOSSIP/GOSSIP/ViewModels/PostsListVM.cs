using GOSSIP.Models;
using GOSSIP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class PostsListVM : ObservableObject
    {
        public ObservableCollection<Post> Posts { get; set; }

        public PostsListVM()
        {
            Posts = [
                new Post(new User("stelmakh_yurii", "stelmakh_yurii.png"), DateTime.Now.AddMinutes(-10), "це пізда", "я їбала, я ваювала, я запускала атамне")
                ];
        }
    }
}

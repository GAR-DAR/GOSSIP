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
    //Список постів. Поки сиро і плачевно
    public class PostsListVM : ObservableObject
    {
        //Колекція постів. Треба підключити до БД
        public ObservableCollection<Post> Posts { get; set; }

        public PostsListVM()
        {
            Posts = [
                new Post(new User("stelmakh_yurii", "stelmakh_yurii.png"), DateTime.Now.AddMinutes(-10), "What C# GUI framework for desktop apps is the best?", "filler text, filler text, filler text, filler text", ["C#", "GUI"], 3, 2),
                new Post(new User("OleksaLviv", "OleksaLviv.png"), DateTime.Now.AddDays(-20), "Can yall share a quality resource about WinAPI?", "...", ["WinAPI", "Windows"], 3, 2)
                ];
        }
    }
}

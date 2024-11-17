using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class MainVM : ObservableObject
    {
        private ObservableObject _selectedVM;
        public ChatsVM ChatsVM = new();
        private PostsListVM PostsListVM = new();

        public ObservableObject SelectedVM
        {
            get
            {
                return _selectedVM;
            }
            set
            {
                _selectedVM = value;
                OnPropertyChanged(nameof(SelectedVM));
            }
        }

        //Tags will be added here
        public ICommand ShowPostsListCommand { get; set; }
        public ICommand ShowChatsCommand { get; set; }

        public MainVM()
        {

            SelectedVM = PostsListVM;
            ShowPostsListCommand = new RelayCommand((obj) => SelectedVM = PostsListVM);
            ShowChatsCommand = new RelayCommand((obj) => SelectedVM = ChatsVM);
        }

    }
}

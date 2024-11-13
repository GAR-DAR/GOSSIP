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
            SelectedVM = new ChatsVM();
            ShowPostsListCommand = new RelayCommand((obj) => SelectedVM = new PostsListVM());
            ShowChatsCommand = new RelayCommand((obj) => SelectedVM = new ChatsVM());
        }

        private void ShowChatsMethod(object obj)
        {
            SelectedVM = new ChatsVM();
        }

    }
}

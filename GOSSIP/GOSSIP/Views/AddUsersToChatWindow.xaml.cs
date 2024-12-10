using GOSSIP.Models;
using GOSSIP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GOSSIP.Views
{
    /// <summary>
    /// Interaction logic for AddUsersToChatWindow.xaml
    /// </summary>
    public partial class AddUsersToChatWindow : Window
    {
        public AddUsersToChatWindow()
        {
            InitializeComponent();
        }

        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var viewModel = DataContext as AddUsersToChatVM;
            if (viewModel != null)
            {
                viewModel.SelectedUsers = listView.SelectedItems.Cast<UserVM>().ToList();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as AddUsersToChatVM;
            if (viewModel != null)
            {
                foreach (UserVM user in e.AddedItems)
                {
                    if (!viewModel.SelectedUsers.Contains(user))
                        viewModel.SelectedUsers.Add(user);
                }

                foreach (UserVM user in e.RemovedItems)
                {
                    viewModel.SelectedUsers.Remove(user);
                }
            }
        }
    }
}

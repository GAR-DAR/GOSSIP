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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GOSSIP.Views
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }
        
        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Отримати елемент ListBoxItem
            if (sender is ListBoxItem item && DataContext is ProfileVM profileVM)
            {
                // Передати вибраний елемент до команди у ViewModel
                if (item.DataContext is TopicVM selectedPost)
                {
                    if (profileVM.DoubleClickCommand.CanExecute(selectedPost))
                        profileVM.DoubleClickCommand.Execute(selectedPost);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
    /// Interaction logic for OpenedChatView.xaml
    /// </summary>
    public partial class OpenedChatView : UserControl
    {
        public OpenedChatView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OpenedChatVM vm = DataContext as OpenedChatVM;

            vm.Messages.CollectionChanged += (s, ev) => ScrollToBottom();


            void ScrollToBottom()
            {
                MessagesListBox.SelectedIndex = MessagesListBox.Items.Count -1;
                MessagesListBox.ScrollIntoView(MessagesListBox.SelectedItem) ;
            }
        }
    }
}

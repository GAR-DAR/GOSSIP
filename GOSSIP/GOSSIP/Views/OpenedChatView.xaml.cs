using GOSSIP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            Loaded += OpenedChatView_Loaded;
        }

        private void OpenedChatView_Loaded(object sender, RoutedEventArgs e)
        {
            if (MessagesListBox.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MessagesListBox.Items.Count > 0)
            {
                MessagesListBox.ScrollIntoView(MessagesListBox.Items[^1]);
            }
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (DataContext is OpenedChatVM vm && vm.SendMessageCommand.CanExecute(null))
            {
                if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    vm.SendMessageCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    vm.PasteEnter(textBox);
                }
            }
        }
    }
}

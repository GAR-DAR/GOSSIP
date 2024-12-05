using GOSSIP.Models;
using GOSSIP.ViewModels;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace GOSSIP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableObject SelectedVM; 

        public SideBarVM ToolBar { get; set; }

        public ChatsVM Chat { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ReplyModel reply = new ReplyModel(
                            1,
                            new UserModel(
                                1,
                                "email",
                                "usename",
                                "password",
                                "Student",
                                "idgaf",
                                "Software Engineering",
                                "Lviv Polytechnic",
                                4,
                                "Bachelor",
                                "User",
                                DateTime.Now,
                                false,
                                "nophotoicon.png",
                                []),
                            null,
                            null,
                            "Reply",
                            DateTime.Now,
                            5,
                            false);

             TopicModel topicModel = new(
                5,
                new UserModel(
                    5,
                    "email",
                    "usename",
                    "password",
                    "Student",
                    "idgaf",
                    "Software Engineering",
                    "Lviv Polytechnic",
                    4,
                    "Bachelor",
                    "User",
                    DateTime.Now,
                    false,
                    "nophotoicon.png",
                    []),
                    "Post",
                    "Idgaf",
                    DateTime.Now,
                    5,
                    [],
                    [
                        reply,
                        new ReplyModel(
                            1,
                            new UserModel(
                                1,
                                "email",
                                "usename",
                                "password",
                                "Student",
                                "idgaf",
                                "Software Engineering",
                                "Lviv Polytechnic",
                                4,
                                "Bachelor",
                                "User",
                                DateTime.Now,
                                false,
                                "nophotoicon.png",
                                []),
                            null,
                            reply,
                            "reply to reply",
                            DateTime.Now,
                            6,
                            false)
                            
                    ],
                    1,
                    false
                );

            string json = JsonSerializer.Serialize(new List<TopicModel>() { topicModel }, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            File.WriteAllText("topic_data.json", json);
            DataContext = new MainVM();
           
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        //-----------------------------------------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

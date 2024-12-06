using GOSSIP.Models;
﻿using GOSSIP.Net;
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
using System.Collections.ObjectModel;

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
         
            Globals.server.Connect();



            //ParentReplyModel parentReplyModel = new ParentReplyModel(
            //            1,
            //            new UserModel(
            //                8,
            //                "email",
            //                "user2",
            //                "password",
            //                "Student",
            //                "idgaf",
            //                "idgaf",
            //                "Lviv Polytechnic",
            //                4,
            //                "Bachelor",
            //                "User",
            //                DateTime.Now,
            //                false,
            //                null,
            //                []),
            //            null,
            //            "content",
            //            DateTime.Now,
            //            5,
            //            false,
            //            [   
            //            ]
            //            );



            //ChildReplyModel child = new ChildReplyModel(
            //                    9,
            //                    new UserModel(
            //                        5,
            //                        "email",
            //                        "username7",
            //                        "password",
            //                        "Student",
            //                        "idgaf",
            //                        "idgaf",
            //                        "Lviv Polytechnic",
            //                        4,
            //                        "Bachelor",
            //                        "User",
            //                        DateTime.Now,
            //                        false,
            //                        null,
            //                        []
            //                    ),
            //                    null,
            //                    "reply to reply",
            //                    DateTime.Now,
            //                    7,
            //                    false,
            //                    null);

            //parentReplyModel.Replies.Add(child);

            //TopicModel topic = new(
            //    6,
            //    new UserModel(8, "email", "user", "password", "Student", "idgaf", "idgaf", "Lviv Polytechnic", 4, "Bachelor", "User", DateTime.Now, false, null, []),
            //    "Topic",
            //    "Content",
            //    DateTime.Now,
            //    5,
            //    [],
            //    [ parentReplyModel
            //    ],
            //    1,
            //    false
            //);

            //parentReplyModel.Topic = topic;
            //child.Topic = topic;

            ////string json = JsonSerializer.Serialize(new ObservableCollection<TopicModel>() { topic }, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve } );
            ////File.WriteAllText("topic_data.json", json);
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
            Globals.server.Disconnect();
            this.Close();
        }
    }
}

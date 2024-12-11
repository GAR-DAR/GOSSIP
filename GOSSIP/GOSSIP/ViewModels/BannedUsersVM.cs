﻿using GOSSIP.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class BannedUsersVM : ObservableObject
    {
        public ICommand CloseCommand { get; set; }
        public ICommand UnbanUserCommand { get; }

        public ObservableCollection<UserVM> BannedUsers { get; }

        public BannedUsersVM()
        {
            JsonStorage storage = new("user_data.json");
            BannedUsers = new(storage.LoadUsers().Where(user => user.IsBanned).Select(user => new UserVM(user)));
            UnbanUserCommand = new RelayCommand(UnbanUserMethod);
        }

        private void UnbanUserMethod(object obj)
        {
            if(obj is UserVM user)
            {
                //Тут пишіть свою реалізацію
                MessageBox.Show($"User {user.Username} was unbanned");
            }
        }

    }
}
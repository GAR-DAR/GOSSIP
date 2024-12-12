using GOSSIP.Net;
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

        public ObservableCollection<UserVM> BannedUsers { get; set; }

        public BannedUsersVM()
        {
            Globals.server.GetBannedUsers();
            Globals.server.getBannedUsersEvent += (users) =>
            {
                BannedUsers = new ObservableCollection<UserVM>(users.Select(u => new UserVM(u)));
                OnPropertyChanged(nameof(BannedUsers));
            };
            UnbanUserCommand = new RelayCommand(UnbanUserMethod);
        }

        private void UnbanUserMethod(object obj)
        {
            if(obj is UserVM user)
            {
                Globals.server.UnbanUser(user.UserModel.ID);
                BannedUsers.Remove(user);
            }
        }

    }
}

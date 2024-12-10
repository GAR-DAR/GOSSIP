using GOSSIP.Models.IDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class UserVM : ObservableObject
    {
        private UserModelID _userModel;
        public UserModelID UserModel
        {
            get => _userModel;
            set
            {
                _userModel = value;
                OnPropertyChanged(nameof(UserModel));
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(Photo));
                OnPropertyChanged(nameof(UserModel.ChatsID));
                SubscribeToChatChanges();

            }
        }

        private void SubscribeToChatChanges()
        {
            foreach (var chat in UserModel.ChatsID)
            {
                // Manually trigger OnPropertyChanged when you know a property has changed
                OnPropertyChanged(nameof(UserModel.ChatsID));
                foreach (var message in chat.Messages)
                {
                    // Manually trigger OnPropertyChanged for messages
                    OnPropertyChanged(nameof(chat.Messages));
                }
            }
        }

        public string Username
        {
            get => UserModel.Username;
            set
            {
                UserModel.Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public DateTime CreatedAt
        {
            get => UserModel.CreatedAt;
            set
            {
                UserModel.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public string Password
        {
            get => UserModel.Password;
            set
            {
                UserModel.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Status
        {
            get => UserModel.Status;
            set
            {
                UserModel.Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string FieldOfStudy
        {
            get => UserModel.FieldOfStudy;
            set
            {
                UserModel.FieldOfStudy = value;
                OnPropertyChanged(nameof(FieldOfStudy));
            }
        }

        public string Specialization
        {
            get => UserModel.Specialization;
            set
            {
                UserModel.Specialization = value;
                OnPropertyChanged(nameof(Specialization));
            }
        }

        public string University
        {
            get => UserModel.University;
            set
            {
                UserModel.University = value;
                OnPropertyChanged(nameof(University));
            }
        }

        public string Degree
        {
            get => UserModel.Degree;
            set
            {
                UserModel.Degree = value;
                OnPropertyChanged(nameof(Degree));
            }
        }

        public string Term
        {
            get => UserModel.Term.ToString();
            set
            {
                if (value != null)
                {
                    UserModel.Term = uint.Parse(value);
                }
                else
                {
                    UserModel.Term = 0;
                }
                OnPropertyChanged(nameof(Term));
            }
        }

        public string Role
        {
            get => UserModel.Role;
            set
            {
                UserModel.Role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public string Photo
        {
            get => UserModel.Photo;
            set
            {
                UserModel.Photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        public string Email
        {
            get => UserModel.Email;
            set
            {
                UserModel.Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public UserVM(UserModelID userModel)
        {
            UserModel = userModel;    

        }
    }
}

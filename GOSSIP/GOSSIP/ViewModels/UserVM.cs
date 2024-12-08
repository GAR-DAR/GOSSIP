using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class UserVM : ObservableObject
    {
        private UserModel _userModel;
        public UserModel UserModel
        {
            get => _userModel;
            set
            {
                _userModel = value;
                OnPropertyChanged(nameof(UserModel));
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(Photo));
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

        public UserVM(UserModel userModel)
        {
            UserModel = userModel;    
        }
    }
}

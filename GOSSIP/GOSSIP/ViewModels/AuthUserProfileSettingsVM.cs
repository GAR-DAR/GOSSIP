
using GOSSIP.Net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class AuthUserProfileSettingsVM : ObservableObject
    {
        public string Header => "Settings";

        private UserVM _user;
        private readonly MainVM _mainVM;

        private int _specializationIndex = -1;
        public int SpecializationIndex
        {
            get => _specializationIndex;
            set
            {
                _specializationIndex = value;
                OnPropertyChanged(nameof(SpecializationIndex));
            }
        }

        private int _universityIndex = -1;
        public int UniversityIndex
        {
            get => _universityIndex;
            set
            {
                _universityIndex = value;
                OnPropertyChanged(nameof(UniversityIndex));
            }
        }

        private int _degreeIndex = -1;
        public int DegreeIndex
        {
            get => _degreeIndex;
            set
            {
                _degreeIndex = value;
                OnPropertyChanged(nameof(DegreeIndex));
            }
        }

        private int _termIndex = -1;
        public int TermIndex
        {
            get => _termIndex;
            set
            {
                _termIndex = value;
                OnPropertyChanged(nameof(TermIndex));
            }
        }

        private bool _isDegreeSelected = false;
        public bool IsDegreeSelected
        {
            get => _isDegreeSelected;
            set
            {
                _isDegreeSelected = value;
                OnPropertyChanged(nameof(IsDegreeSelected));

                // Очищення значення, якщо Degree не вибрано
                if (!_isDegreeSelected)
                {
                    TermIndex = -1;
                    TermsOptions.Clear();
                    return;
                }

                // Оновлення TermsOptions відповідно до Degree
                TermsOptions.Clear(); // Очищаємо колекцію перед додаванням нових значень
                FillTermsCollection();
                OnPropertyChanged(nameof(TermsOptions));
            }
        }


        private bool _isStudentOrFaculty;
        public bool IsStudentOrFaculty
        {
            get => _isStudentOrFaculty;
            set
            {
                _isStudentOrFaculty = value;
                OnPropertyChanged(nameof(IsStudentOrFaculty));
                if (!_isStudentOrFaculty)
                {
                    SpecializationIndex = -1;
                    UniversityIndex = -1;
                    DegreeIndex = -1;
                    TermIndex = -1;
                }
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                IsStudentOrFaculty = value == "Student" || value == "Faculty";
            }
        }

        private string _fieldOfStudy;
        public string FieldOfStudy
        {
            get => _fieldOfStudy;
            set
            {
                _fieldOfStudy = value;
                OnPropertyChanged(nameof(FieldOfStudy));
            }
        }

        private string _specialization;
        public string Specialization
        {
            get => _specialization;
            set
            {
                _specialization = value;
                OnPropertyChanged(nameof(Specialization));
            }
        }

        private string _university;
        public string University
        {
            get => _university;
            set
            {
                _university = value;
                OnPropertyChanged(nameof(University));
            }
        }

        private string _degree;
        public string Degree
        {
            get => _degree;
            set
            {
                _degree = value;
                OnPropertyChanged(nameof(Degree));
                IsDegreeSelected = value != null; 
            }
        }

        private string _term;
        public string Term
        {
            get => _term;
            set
            {
                _term = value;
                OnPropertyChanged(nameof(Term));
            }
        }

        private string _photo;
        public string Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private bool _isSaved;
        public bool IsSaved
        {
            get => _isSaved;
            set
            {
                _isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
            }
        }


        public ICommand SaveChangesCommand { get; }
        public ICommand ChangePhotoCommand { get; }

        public List<string> StatusOptions { get; set; } = [];
        public List<string> FieldOfStudyOptions { get; set; } = [];
        public List<string> SpecializationOptions { get; set; } = [];
        public List<string> UniversityOptions { get; set; } = [];
        public List<string> DegreeOptions { get; set; } = [];

        public ObservableCollection<string> TermsOptions { get; set; } = [];


        private void FillTermsCollection()
        {
            TermsOptions.Clear();

            if(Degree == "Bachelor")
            {
                foreach (var term in new[] { "1", "2", "3", "4" }) TermsOptions.Add(term);
            }
            else if (Degree == "Master")
            {
                foreach (var term in new[] { "1", "2" }) TermsOptions.Add(term);
            }
            else if (Degree == "Postgraduate")
            {
                foreach (var term in new[] { "1", "2", "3", "4" }) TermsOptions.Add(term);
            }
            else if (Degree == "PhD")
            {
                foreach (var term in new[] { "1", "2", "3", "4" }) TermsOptions.Add(term);
            }
        }

        private ObservableCollection<string> _universityShortenOptions;

        public ObservableCollection<string> UniversityShortenOptions
        {
            get => _universityShortenOptions;
            set
            {
                _universityShortenOptions = value;
                OnPropertyChanged(nameof(UniversityShortenOptions));
            }
        }

        public AuthUserProfileSettingsVM(UserVM user, MainVM mainVM)
        {
            Globals.server.GetInformationForSignUp();

            Globals.server.getStatusesEvent += getStatuses;
            Globals.server.getFieldOfStudyEvent += getFieldOfStudy;
            Globals.server.getUniversitiesEvent += getUniversityOptions;
            Globals.server.getSpecializationsEvent += getSpecializationOptions;
            Globals.server.getDegreesEvent += getDegreeOptions;

            _mainVM = mainVM;
            _user = user;
            Username = user.Username;
            Status = user.Status;
            FieldOfStudy = user.FieldOfStudy;
            Specialization = user.Specialization;
            University = user.University;
            Degree = user.Degree;
            Term = user.Term.ToString();
            Photo = user.Photo;
            Email = user.Email;
            Password = user.Password;

            FillTermsCollection();

            IsStudentOrFaculty = user.Status == "Student" || user.Status == "Faculty";
            SpecializationIndex = SpecializationOptions.IndexOf(Specialization);
            UniversityIndex = UniversityOptions.IndexOf(University);
            DegreeIndex = DegreeOptions.IndexOf(Degree);
            TermIndex = TermsOptions.IndexOf(Term.ToString());

            SaveChangesCommand = new RelayCommand(SaveChangesMethod);

            Globals.server.refreshUser += (user) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Photo = user.Photo;
                });
            };

            ChangePhotoCommand = new RelayCommand(obj => {
                /*TODO: Додати зміну фото*/

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                if (openFileDialog.ShowDialog() == true)
                {
                    Photo = openFileDialog.FileName;

                    // Proceed to upload the file to the FTP server
                    string ftpUrl = "ftp://ftp.byethost7.com/htdocs/Icons/" + Path.GetFileName(Photo);
                    string ftpUsername = "b7_37868429"; // Replace with your FTP username
                    string ftpPassword = "hello12_"; // Replace with your FTP password

                    // Create an FtpWebRequest
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    // Read the contents of the file into a byte array
                    byte[] fileContents = File.ReadAllBytes(Photo);

                    request.ContentLength = fileContents.Length;

                    // Upload the file
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(fileContents, 0, fileContents.Length);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                    }

                }

                _user.UserModel.Photo = "http://gossip.byethost7.com/Icons/" + Path.GetFileName(Photo);
                MainVM.AuthorizedUserVM.Photo = "http://gossip.byethost7.com/Icons/" + Path.GetFileName(Photo);
                Globals.server.SendPacket(SignalsEnum.ChangeUserPhoto, _user.UserModel);

            });
        }

        private void getDegreeOptions(List<string> degreeOptions)
        {
            DegreeOptions = degreeOptions;
            OnPropertyChanged(nameof(DegreeOptions));
        }

        private void getUniversityOptions(List<string> universityOptions)
        {
            UniversityOptions = universityOptions;

            UniversityShortenOptions = new ObservableCollection<string>(
                                         universityOptions.Select(x => x.Length > 30 ? x.Substring(0, 30) + "..." : x).ToList()
    );


            OnPropertyChanged(nameof(UniversityOptions));

        }

        private void getSpecializationOptions(List<string> specializationOptions)
        {
            SpecializationOptions = specializationOptions;
            OnPropertyChanged(nameof(SpecializationOptions));
        }

        private void getFieldOfStudy(List<string> fieldsOfStudy)
        {
            FieldOfStudyOptions = fieldsOfStudy;
            OnPropertyChanged(nameof(FieldOfStudyOptions));
        }

        private void getStatuses(List<string> statuses)
        {
            StatusOptions = statuses;
            OnPropertyChanged(nameof(StatusOptions));
        }


        private void SaveChangesMethod(object obj)
        {
            _user.Username = Username;
            _user.Status = Status;
            _user.FieldOfStudy = FieldOfStudy;

            if (IsStudentOrFaculty)
            {
                _user.Specialization = Specialization;
                _user.University = University;
                _user.Degree = Degree;
                _user.Term = Term;
            }
            else
            {
                _user.Specialization = null;
                _user.University = null;
                _user.Degree = null;
                _user.Term = null;
            }

            _user.Email = Email;

            if(!string.IsNullOrEmpty(Password))
            {
                _user.Password = Password;
            }


            Globals.server.EditUser(new UserModelID(_user.UserModel));
            Globals.server.editUserEvent += (user) => OnEditUserSuccess(new UserVM(user));

        }

        private void OnEditUserSuccess(UserVM user)
        {
            _user = user;
            IsSaved = true;
        }
    }
}

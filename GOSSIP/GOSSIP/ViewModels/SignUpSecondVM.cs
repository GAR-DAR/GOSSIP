
using GOSSIP.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    //Другий VM реєстрації. Містить другорядну інформацію: статус, галузь знань, спеціальність та університет
    public class SignUpSecondVM : ObservableObject
    {
        //Копія головної VM реєстрації
        private readonly SignUpMainVM _mainVM;

        public ICommand BackCommand { get; set; }
        public ICommand CompleteSignUpCommand { get; set; }

        
       
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
                University = UniversityOptions[_universityIndex];
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

        private bool _allFieldsAreNotFilled;
        public bool AllFieldsAreNotFilled
        {
            get => _allFieldsAreNotFilled;
            set
            {
                _allFieldsAreNotFilled = value;
                OnPropertyChanged(nameof(AllFieldsAreNotFilled));
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
                switch (Degree)
                {
                    case "Bachelor":
                        foreach (var term in new[] { "1", "2", "3", "4" }) TermsOptions.Add(term);
                        break;
                    case "Master":
                        foreach (var term in new[] { "1", "2" }) TermsOptions.Add(term);
                        break;
                    case "Postgraduate":
                        foreach (var term in new[] { "1", "2", "3", "4"}) TermsOptions.Add(term);
                        break;
                    case "PhD":
                        foreach (var term in new[] { "1", "2", "3", "4" }) TermsOptions.Add(term);
                        break;
                }
                OnPropertyChanged(nameof(TermsOptions));
            }
        }


        private bool _isStudentOrFaculty = false;
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

        public string Status
        {
            get => _mainVM.Status;
            set
            {
                _mainVM.Status = value;
                OnPropertyChanged(nameof(Status));
                IsStudentOrFaculty = value == "Student" || value == "Faculty";
            }
        }

        public string FieldOfStudy
        {
            get => _mainVM.FieldOfStudy;
            set
            {
                _mainVM.FieldOfStudy = value;
                OnPropertyChanged(nameof(FieldOfStudy));
            }
        }

        public string Specialization
        {
            get => _mainVM.Specialization;
            set
            {
                _mainVM.Specialization = value;
                OnPropertyChanged(nameof(Specialization));
            }
        }

        public string University
        {
            get => _mainVM.University;
            set
            {
                _mainVM.University = value;
                OnPropertyChanged(nameof(University));
                
            }
        }

        public string Degree
        {
            get => _mainVM.Degree;
            set
            {
                _mainVM.Degree = value;
                OnPropertyChanged(nameof(Degree));
                IsDegreeSelected = value != null; 
            }
        }

        public uint Term
        {
            get => _mainVM.Term;
            set
            {
                _mainVM.Term = value;
                OnPropertyChanged(nameof(Term));
            }
        }

        public event Action<UserVM> CloseDialog;

        
        public List<string> StatusOptions { get; set; } = [];
        public List<string> FieldOfStudyOptions { get; set; } = [];
        public List<string> SpecializationOptions { get; set; } = [];
        public List<string> UniversityOptions { get; set; } = [];
        public List<string> DegreeOptions { get; set; } = [];

        public ObservableCollection<string> TermsOptions { get; set; } = [];
        
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


        public SignUpSecondVM(SignUpMainVM signUpMainVM)
        {
            _mainVM = signUpMainVM;
            BackCommand = new RelayCommand((obj) => _mainVM.SelectedVM = _mainVM.SignUpFirstVM);
            CompleteSignUpCommand = new RelayCommand(CompleteSignUpMethod);

            Globals.server.GetInformationForSignUp();

            Globals.server.getStatusesEvent += getStatuses;
            Globals.server.getFieldOfStudyEvent += getFieldOfStudy;
            Globals.server.getUniversitiesEvent += getUniversityOptions;
            Globals.server.getSpecializationsEvent += getSpecializationOptions;
            Globals.server.getDegreesEvent += getDegreeOptions;
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

        private void CompleteSignUpMethod(object obj)
        {
            try
            {
                ValidateInput();

                UserModel newUser = new UserModel(
                    id: 0,
                    email: _mainVM.Email,
                    username: _mainVM.Username,
                    password: _mainVM.Password,
                    status: _mainVM.Status,
                    fieldOfStudy: _mainVM.FieldOfStudy,
                    specialization: _mainVM.Specialization,
                    university: _mainVM.University,
                    term: _mainVM.Term,
                    degree: _mainVM.Degree,
                    role: "User",
                    createdAt: DateTime.Now,
                    isBanned: false,
                    photo: "pack://application:,,,/Resources/Images/TempUserIcons/nophotoicon.png",
                    chats: []
                );

                Globals.server.SignUp(new UserModelID(newUser));
                Globals.server.signUpEvent += (user) => OnSignUpSuccess(new UserVM(user));

                CloseDialog.Invoke(new(newUser));
            }
            catch (Exception ex)
            {
            }
        }
        private void OnSignUpSuccess(UserVM user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CloseDialog.Invoke(user);
            });
        }
        private void ValidateInput()
        {
            if (string.IsNullOrEmpty(Status) || string.IsNullOrEmpty(FieldOfStudy) ||
                ((Status == "Student" || Status == "Faculty") &&
                (string.IsNullOrEmpty(Specialization) || string.IsNullOrEmpty(Degree) || Term == 0)))
            {
                AllFieldsAreNotFilled = true;
                throw new ArgumentException("Please fill in all fields.");
            }

            AllFieldsAreNotFilled = false;    
        }

    }
}

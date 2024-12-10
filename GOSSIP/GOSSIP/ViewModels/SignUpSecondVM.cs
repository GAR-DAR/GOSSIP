﻿using GOSSIP.Models.IDModels;
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

        private readonly ChatService _chatService = new("user_data.json");

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

        public uint? Term
        {
            get => _mainVM.Term;
            set
            {
                _mainVM.Term = value;
                OnPropertyChanged(nameof(Term));
            }
        }

        public event Action<UserVM> CloseDialog;

        //Статуси, галузі знань, спеціальності та університети. Потім (я так розумію) буде приєднано до БД.
        //public List<string> StatusOptions { get; set; } = ["Student", "Faculty", "Learner", "None"];
        //public List<string> FieldOfStudyOptions { get; set; } =
        //[
        //    "1. Education",
        //    "2. Arts & Culture",
        //    "3. Human Sciences",
        //    "4. Religion & Theology",
        //    "5. Social Sciences",
        //    "6. Journalism",
        //    "7. Management & Administration",
        //    "8. Law",
        //    "9. Biology",
        //    "10. Natural Sciences",
        //    "11. Mathematics & Statistics",
        //    "12. Information Technology",
        //    "13. Mechanical Engineering",
        //    "14. Electrical Engineering",
        //    "15. Cat Sciences",
        //    "16. Chemical Engineering & Bioengineering",
        //    "17. Electronics & Automation",
        //    "18. Production & Technology",
        //    "19. Architecture & Building",
        //    "20. Agricultural Sciences",
        //    "21. Veterinary",
        //    "22. Healthcare",
        //    "23. Social Work",
        //    "24. Service Sector",
        //    "25. Military & Defence",
        //    "26. Civil Security",
        //    "27. Transport",
        //    "28. Prompt Engineering",
        //    "29. International Relations"
        //];
        //public List<string> SpecializationOptions { get; set; } = ["Software engineering", "Computer Science", "System Analisys"];
        //public List<string> UniversityOptions { get; set; } = ["Lviv Polytechnic", "elenu", "Lviv National Forestry University", "Kyiv Polytechnic Institute", "Taras Shevchenko National University of Kyiv"];
        //public List<string> DegreeOptions { get; set; } = ["Bachelor", "Master", "Postgraduate", "PhD"];
        public List<string> StatusOptions { get; set; } = [];
        public List<string> FieldOfStudyOptions { get; set; } = [];
        public List<string> SpecializationOptions { get; set; } = [];
        public List<string> UniversityOptions { get; set; } = [];
        public List<string> DegreeOptions { get; set; } = [];

        public ObservableCollection<string> TermsOptions { get; set; } = [];

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

                UserModelID newUser = new UserModelID(
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
                    chatsID: []
                );

                Globals.server.SignUp(newUser);
                Globals.server.signUpEvent += (user) => OnSignUpSuccess(new UserVM(user));

                _chatService.AddUser(newUser);
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

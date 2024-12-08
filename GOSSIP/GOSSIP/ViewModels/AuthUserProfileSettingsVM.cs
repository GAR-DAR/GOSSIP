using GOSSIP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.ViewModels
{
    public class AuthUserProfileSettingsVM : ObservableObject
    {
        public string Header => "Settings";

        private readonly UserModel _user;

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

        private int _term;
        public int Term
        {
            get => _term;
            set
            {
                _term = value;
                OnPropertyChanged(nameof(Term));
            }
        }

        //Статуси, галузі знань, спеціальності та університети. Потім (я так розумію) буде приєднано до БД.
        public List<string> StatusOptions { get; set; } = ["Student", "Faculty", "Learner", "None"];
        public List<string> FieldOfStudyOptions { get; set; } =
        [
            "1. Education",
            "2. Arts & Culture",
            "3. Human Sciences",
            "4. Religion & Theology",
            "5. Social Sciences",
            "6. Journalism",
            "7. Management & Administration",
            "8. Law",
            "9. Biology",
            "10. Natural Sciences",
            "11. Mathematics & Statistics",
            "12. Information Technology",
            "13. Mechanical Engineering",
            "14. Electrical Engineering",
            "15. Cat Sciences",
            "16. Chemical Engineering & Bioengineering",
            "17. Electronics & Automation",
            "18. Production & Technology",
            "19. Architecture & Building",
            "20. Agricultural Sciences",
            "21. Veterinary",
            "22. Healthcare",
            "23. Social Work",
            "24. Service Sector",
            "25. Military & Defence",
            "26. Civil Security",
            "27. Transport",
            "28. Prompt Engineering",
            "29. International Relations"
        ];
        public List<string> SpecializationOptions { get; set; } = ["Software Engineering", "Computer Science", "System Analisys"];
        public List<string> UniversityOptions { get; set; } = ["Lviv Polytechnic", "elenu", "Lviv National Forestry University", "Kyiv Polytechnic Institute", "Taras Shevchenko National University of Kyiv"];
        public List<string> DegreeOptions { get; set; } = ["Bachelor", "Master", "Postgraduate", "PhD"];
        public ObservableCollection<string> TermsOptions { get; set; } = [];

        public AuthUserProfileSettingsVM(UserModel user)
        {
            _user = user;
            Status = user.Status;
            FieldOfStudy = user.FieldOfStudy;
            Specialization = user.Specialization;
            University = user.University;
            Degree = user.Degree;
            Term = user.Term;
            IsStudentOrFaculty = user.Status == "Student" || user.Status == "Faculty";
            SpecializationIndex = SpecializationOptions.IndexOf(Specialization);
            UniversityIndex = UniversityOptions.IndexOf(University);
            DegreeIndex = DegreeOptions.IndexOf(Degree);
            TermIndex = TermsOptions.IndexOf(Term.ToString());
        }
    }
}

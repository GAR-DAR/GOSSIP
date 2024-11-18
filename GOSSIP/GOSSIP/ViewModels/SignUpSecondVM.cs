using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GOSSIP.ViewModels
{
    public class SignUpSecondVM : ObservableObject
    {
        private readonly SignUpMainVM _mainVM;

        public ICommand BackCommand { get; set; }
        public ICommand CompleteSignUpCommand { get; set; }

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
        public List<string> SpecializationOptions { get; set; } = ["Software engineering", "Computer Science", "System Analisys"];
        public List<string> UniversityOptions { get; set; } = ["Lviv Polytechnic", "elenu", "Lviv National Forestry University", "Kyiv Polytechnic Institute", "Taras Shevchenko National University of Kyiv"];

        public SignUpSecondVM(SignUpMainVM signUpMainVM)
        {
            _mainVM = signUpMainVM;
            BackCommand = new RelayCommand((obj) => _mainVM.SelectedVM = _mainVM.SignUpFirstVM);
        }

    }
}

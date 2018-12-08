using PrettigLokaal.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class RegisterPageViewModel : ViewModelBase
    {
        private string email = "";
        private string fullName = "";
        private string password = "";
        private string repeatPassword = "";
        private DateTime birthDate = DateTime.Now;

        public string Email { get { return email; } set { email = value; RaisePropertyChanged(); } }
        public string FullName { get { return fullName; } set { fullName = value; RaisePropertyChanged(); } }
        public string Password { get { return password; } set { password = value; RaisePropertyChanged(); } }
        public string RepeatPassword { get { return repeatPassword; } set { repeatPassword = value; RaisePropertyChanged(); } }
        public DateTime BirthDate { get { return birthDate; } set { birthDate = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(Email))
                ModelErrors["Email"] = "Gelieve een geldig E-Mailadres in te voeren.";
            if (string.IsNullOrWhiteSpace(FullName))
                ModelErrors["FullName"] = "Gelieve uw naam in te voeren.";

            if (string.IsNullOrWhiteSpace(Password))
                ModelErrors["Password"] = "Gelieve een geldig wachtwoord in te voeren.";
            if (string.IsNullOrWhiteSpace(RepeatPassword))
                ModelErrors["RepeatPassword"] = "Gelieve uw wachtwoord nogmaals in te voeren.";

            if (Password != null && RepeatPassword != null)
                if (Password != RepeatPassword)
                {
                    ModelErrors["Password"] = "De wachtwoorden komen niet overeen.";
                    ModelErrors["RepeatPassword"] = "De wachtwoorden komen niet overeen.";
                }

            if ((DateTime.Now - BirthDate).Days < 12 * 365 + 4)
                ModelErrors["BirthDate"] = "U moet tenminste 12 jaar oud zijn om u te registreren.";
        }
    }
}

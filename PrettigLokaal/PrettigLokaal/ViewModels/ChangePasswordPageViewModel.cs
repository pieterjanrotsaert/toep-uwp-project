using PrettigLokaal.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class ChangePasswordPageViewModel : ViewModelBase 
    {
        private string oldPassword = "";
        private string password = "";
        private string repeatPassword = "";

        public string OldPassword { get { return oldPassword; } set { oldPassword = value; RaisePropertyChanged(); } }
        public string Password { get { return password; } set { password = value; RaisePropertyChanged(); } }
        public string RepeatPassword { get { return repeatPassword; } set { repeatPassword = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(OldPassword))
                ModelErrors["OldPassword"] = "Gelieve uw oud wachtwoord in te voeren.";

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
        }
    }
}

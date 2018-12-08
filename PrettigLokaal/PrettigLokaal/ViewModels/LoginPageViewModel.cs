using PrettigLokaal.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class LoginPageViewModel : ViewModelBase 
    {
        private string email = "";
        private string password = "";
        private bool remember = false;

        public string Email { get { return email; } set { email = value; RaisePropertyChanged(); } }
        public string Password { get { return password; } set { password = value; RaisePropertyChanged(); } }
        public bool Remember { get { return remember; } set { remember = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(Email))
                ModelErrors["Email"] = "Gelieve een geldig E-Mailadres in te voeren.";
            if (string.IsNullOrWhiteSpace(Password))
                ModelErrors["Password"] = "Gelieve een geldig wachtwoord in te voeren.";
        }
    }
}

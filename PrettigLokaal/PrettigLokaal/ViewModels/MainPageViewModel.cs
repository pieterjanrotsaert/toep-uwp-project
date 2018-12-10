using PrettigLokaal.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class MainPageViewModel : ViewModelBase 
    {

        private string title;
        private bool isMerchant = false;
        private bool isLoggedIn = false;
        private bool isLoading = false;
        private bool showMerchantSignup = false;
        private bool canGoBack = false;

        public string Title { get { return title; } set { title = value; RaisePropertyChanged(); } }
        public bool IsMerchant { get { return isMerchant; } set { isMerchant = value; RaisePropertyChanged(); } }
        public bool IsLoggedIn { get { return isLoggedIn; } set { isLoggedIn = value; RaisePropertyChanged(); } }
        public bool IsLoading { get { return isLoading; } set { isLoading = value; RaisePropertyChanged(); } }
        public bool ShowMerchantSignup { get { return showMerchantSignup; } set { showMerchantSignup = value; RaisePropertyChanged(); } }
        public bool CanGoBack { get { return canGoBack; } set { canGoBack = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            // Nothing to do.
        }
    }
}

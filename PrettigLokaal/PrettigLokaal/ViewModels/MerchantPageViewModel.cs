using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class MerchantPageViewModel : ViewModelBase
    {
        private Merchant merchant;
        private bool isFollowing;
        private bool hasFacebook;
        private string tagLine;
        private string openingHours;

        public Merchant Merchant { get { return merchant; } set { merchant = value; RaisePropertyChanged(); } }
        public bool IsFollowing { get { return isFollowing; } set { isFollowing = value; RaisePropertyChanged(); } }
        public bool HasFacebook { get { return hasFacebook; } set { hasFacebook = value; RaisePropertyChanged(); } }
        public string TagLine { get { return tagLine; } set { tagLine = value; RaisePropertyChanged(); } }
        public string OpeningHours { get { return openingHours; } set { openingHours = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
        }
    }
}

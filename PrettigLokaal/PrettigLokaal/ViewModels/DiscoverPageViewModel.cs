using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class DiscoverPageViewModel : ViewModelBase
    {
        private List<Merchant> featuredMerchants;
        private List<Merchant> recentlyAddedMerchants;
        private bool isLoading = true;

        public List<Merchant> FeaturedMerchants { get { return featuredMerchants; } set { featuredMerchants = value; RaisePropertyChanged(); } }
        public List<Merchant> RecentlyAddedMerchants { get { return recentlyAddedMerchants; } set { recentlyAddedMerchants = value; RaisePropertyChanged(); } }
        public bool IsLoading { get { return isLoading; } set { isLoading = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            throw new NotImplementedException();
        }
    }
}

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
        private List<Merchant> eventPromotionMerchants;
        private List<Event> events;
        private List<Promotion> promotions;
        private bool isLoading = true;

        public List<Merchant> FeaturedMerchants { get { return featuredMerchants; } set { featuredMerchants = value; RaisePropertyChanged(); } }
        public List<Merchant> RecentlyAddedMerchants { get { return recentlyAddedMerchants; } set { recentlyAddedMerchants = value; RaisePropertyChanged(); } }
        public List<Merchant> EventPromotionMerchants { get { return eventPromotionMerchants; } set { eventPromotionMerchants = value; RaisePropertyChanged(); } }
        public List<Event> Events { get { return events; } set { events = value; RaisePropertyChanged(); } }
        public List<Promotion> Promotions { get { return promotions; } set { promotions = value; RaisePropertyChanged(); } }
        public bool IsLoading { get { return isLoading; } set { isLoading = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            throw new NotImplementedException();
        }
    }
}

using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class FeedPageViewModel : ViewModelBase
    {
        private List<Promotion> promotions;
        private List<Event> events;
        private List<Merchant> featuredMerchants;
        private List<Merchant> followedMerchants;
        private bool notFollowing;

        public List<Promotion> Promotions { get { return promotions; } set { promotions = value; RaisePropertyChanged(); } }
        public List<Event> Events { get { return events; } set { events = value; RaisePropertyChanged(); } }
        public List<Merchant> FeaturedMerchants { get { return featuredMerchants; } set { featuredMerchants = value; RaisePropertyChanged(); } }
        public List<Merchant> FollowedMerchants { get { return followedMerchants; } set { followedMerchants = value; RaisePropertyChanged(); } }
        public bool NotFollowing { get { return notFollowing; } set { notFollowing = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            
        }
    }
}

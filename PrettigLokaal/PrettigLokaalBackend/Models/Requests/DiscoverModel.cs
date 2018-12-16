using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class DiscoverModel
    {
        public List<Merchant> FeaturedMerchants { get; set; }
        public List<Merchant> RecentlyAddedMerchants { get; set; }
        public List<Promotion> Promotions { get; set; }
        public List<Event> Events { get; set; }
        public List<Merchant> EventPromotionMerchants { get; set; }
    }
}

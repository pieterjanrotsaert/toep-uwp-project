using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.DomainModels
{
    public class Merchant
    {
        public int Id { get; set; }
        public Account Account { get; set; } // The account this merchant belongs to.

        public string PhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public List<MerchantSubscription> Subscriptions { get; set; }
        public List<Image> Images { get; set; }
        public List<Event> Events { get; set; }
        public List<Promotion> Promotions { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Merchant
    {
        public int Id { get; set; }
        [JsonIgnore] public Account Account { get; set; } // The account this merchant belongs to.

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public List<OpeningHourSpan> OpeningHours { get; set; }
        public List<Tag> Tags { get; set; }

        [JsonIgnore] public List<MerchantSubscription> Subscriptions { get; set; }
        [JsonIgnore] public List<Image> Images { get; set; }
        [JsonIgnore] public List<Event> Events { get; set; }
        [JsonIgnore] public List<Promotion> Promotions { get; set; }
    }
}

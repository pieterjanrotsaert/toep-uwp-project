using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Image Image { get; set; }
        [JsonIgnore] public Merchant Organizer { get; set; }
        public bool HasCouponCode { get; set; } = false;
        public string Description { get; set; }
    }
}

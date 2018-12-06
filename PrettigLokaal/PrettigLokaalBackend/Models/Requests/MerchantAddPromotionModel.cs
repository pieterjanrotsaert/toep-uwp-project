using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class MerchantAddPromotionModel
    {
        public int Id { get; set; } = -1;

        [Required] public string Name;
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [Required] public string Description { get; set; }
        [Required] public bool HasCouponCode { get; set; }

        public Image Image { get; set; }
        
    }
}

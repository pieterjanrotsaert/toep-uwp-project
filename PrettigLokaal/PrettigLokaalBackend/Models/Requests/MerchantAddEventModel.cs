using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class MerchantAddEventModel
    {
        public int Id { get; set; } = -1;

        [Required] public string Name;
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string PlaceDescription { get; set; }

        public Image Image { get; set; }
    }
}

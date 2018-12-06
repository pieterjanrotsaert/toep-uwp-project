using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Event
    {
        public int Id { get; set; }
        public string Name;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Image Image { get; set; }
        [JsonIgnore] public Merchant Organizer { get; set; }
        public string Description { get; set; }
        public string PlaceDescription { get; set; }
    }
}

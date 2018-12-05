using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.DomainModels
{
    public class Event
    {
        public int Id { get; set; }
        public string Name;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Image Image { get; set; }
        public Merchant Organizer { get; set; }
        public string Description { get; set; }
    }
}

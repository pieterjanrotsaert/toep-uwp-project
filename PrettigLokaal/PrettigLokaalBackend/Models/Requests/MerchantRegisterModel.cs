using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class MerchantRegisterModel
    {
        [Required] public string Name { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public string ContactEmail { get; set; }
        [Required] public string Address { get; set; }
        [Required] public string Description { get; set; }
        [Required] public List<string> Tags { get; set; }

        [Required] public DateTime OpenTimeMonday { get; set; }
        [Required] public DateTime OpenTimeTuesday { get; set; }
        [Required] public DateTime OpenTimeWednesday { get; set; }
        [Required] public DateTime OpenTimeThursday { get; set; }
        [Required] public DateTime OpenTimeFriday { get; set; }
        [Required] public DateTime OpenTimeSaturday { get; set; }
        [Required] public DateTime OpenTimeSunday { get; set; }

        [Required] public DateTime CloseTimeMonday { get; set; }
        [Required] public DateTime CloseTimeTuesday { get; set; }
        [Required] public DateTime CloseTimeWednesday { get; set; }
        [Required] public DateTime CloseTimeThursday { get; set; }
        [Required] public DateTime CloseTimeFriday { get; set; }
        [Required] public DateTime CloseTimeSaturday { get; set; }
        [Required] public DateTime CloseTimeSunday { get; set; }

        public List<OpeningHourSpan> GetOpeningHours()
        {
            List<OpeningHourSpan> spans = new List<OpeningHourSpan>();
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeMonday, CloseTime = CloseTimeMonday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeTuesday, CloseTime = CloseTimeTuesday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeWednesday, CloseTime = CloseTimeWednesday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeThursday, CloseTime = CloseTimeThursday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeFriday, CloseTime = CloseTimeFriday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeSaturday, CloseTime = CloseTimeSaturday });
            spans.Add(new OpeningHourSpan() { OpenTime = OpenTimeSunday, CloseTime = CloseTimeSunday });
            return spans;
        }
    }
}

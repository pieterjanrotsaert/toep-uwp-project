using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PrettigLokaal.ViewModels
{
    class MerchantRegisterPageViewModel : ViewModelBase
    {
        private bool isMerchant;

        private string name = "";
        private string phoneNumber = "";
        private string contactEmail = "";
        private string address = "";
        private string description = "";
        private string tagList = "";
        private string facebookLink = "";

        private DateTime openTimeMonday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeTuesday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeWednesday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeThursday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeFriday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeSaturday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
        private DateTime openTimeSunday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);

        private DateTime closeTimeMonday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeTuesday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeWednesday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeThursday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeFriday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeSaturday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);
        private DateTime closeTimeSunday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 18, 0, 0);

        public bool IsMerchant { get { return isMerchant; } set { isMerchant = value; RaisePropertyChanged(); } }

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }
        public string PhoneNumber { get { return phoneNumber; } set { phoneNumber = value; RaisePropertyChanged(); } }
        public string ContactEmail { get { return contactEmail; } set { contactEmail = value; RaisePropertyChanged(); } }
        public string Address { get { return address; } set { address = value; RaisePropertyChanged(); } }
        public string Description { get { return description; } set { description = value; RaisePropertyChanged(); } }
        public string TagList { get { return tagList; } set { tagList = value; RaisePropertyChanged(); } }
        public string FacebookLink { get { return facebookLink; } set { facebookLink = value; RaisePropertyChanged(); } }

        public DateTime OpenTimeMonday { get { return openTimeMonday; } set { openTimeMonday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeTuesday { get { return openTimeTuesday; } set { openTimeTuesday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeWednesday { get { return openTimeWednesday; } set { openTimeWednesday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeThursday { get { return openTimeThursday; } set { openTimeThursday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeFriday { get { return openTimeFriday; } set { openTimeFriday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeSaturday { get { return openTimeSaturday; } set { openTimeSaturday = value; RaisePropertyChanged(); } }
        public DateTime OpenTimeSunday { get { return openTimeSunday; } set { openTimeSunday = value; RaisePropertyChanged(); } }

        public DateTime CloseTimeMonday { get { return closeTimeMonday; } set { closeTimeMonday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeTuesday { get { return closeTimeTuesday; } set { closeTimeTuesday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeWednesday { get { return closeTimeWednesday; } set { closeTimeWednesday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeThursday { get { return closeTimeThursday; } set { closeTimeThursday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeFriday { get { return closeTimeFriday; } set { closeTimeFriday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeSaturday { get { return closeTimeSaturday; } set { closeTimeSaturday = value; RaisePropertyChanged(); } }
        public DateTime CloseTimeSunday { get { return closeTimeSunday; } set { closeTimeSunday = value; RaisePropertyChanged(); } }

        private List<OpeningHourSpan> GetOpeningHours()
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

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(Name))
                ModelErrors["Name"] = "Dit veld is vereist.";
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                ModelErrors["PhoneNumber"] = "Dit veld is vereist.";
            if (string.IsNullOrWhiteSpace(ContactEmail))
                ModelErrors["ContactEmail"] = "Dit veld is vereist.";
            if (string.IsNullOrWhiteSpace(Address))
                ModelErrors["Address"] = "Dit veld is vereist.";
            if (string.IsNullOrWhiteSpace(Description))
                ModelErrors["Description"] = "Dit veld is vereist.";
            if (string.IsNullOrWhiteSpace(TagList))
                ModelErrors["TagList"] = "Gelieve tenminste 1 tag in te geven.";

            if(!string.IsNullOrWhiteSpace(FacebookLink))
            {
                if(!FacebookLink.Contains("facebook.com/"))
                    ModelErrors["FacebookLink"] = "Voer een geldige facebooklink in. (bv: 'www.facebook.com/pagina')";
            }
        }
    }
}

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
        private string selectedImageFile = "Geen bestand geselecteerd.";
        private string imageData = null; // Base64 imagedata
        private bool hasImage = false;
        public StorageFile selectedFile;

        private DateTime openTimeMonday = DateTime.Now;
        private DateTime openTimeTuesday = DateTime.Now;
        private DateTime openTimeWednesday = DateTime.Now;
        private DateTime openTimeThursday = DateTime.Now;
        private DateTime openTimeFriday = DateTime.Now;
        private DateTime openTimeSaturday = DateTime.Now;
        private DateTime openTimeSunday = DateTime.Now;

        private DateTime closeTimeMonday = DateTime.Now;
        private DateTime closeTimeTuesday = DateTime.Now;
        private DateTime closeTimeWednesday = DateTime.Now;
        private DateTime closeTimeThursday = DateTime.Now;
        private DateTime closeTimeFriday = DateTime.Now;
        private DateTime closeTimeSaturday = DateTime.Now;
        private DateTime closeTimeSunday = DateTime.Now;

        public StorageFile SelectedFile { get { return selectedFile; } set { selectedFile = value; RaisePropertyChanged(); } }
        public bool HasImage { get { return hasImage; } }

        public bool IsMerchant { get { return isMerchant; } set { isMerchant = value; RaisePropertyChanged(); } }

        public string SelectedImageFile { get { return selectedImageFile; } set { selectedImageFile = value; RaisePropertyChanged(); } }
        public string ImageData
        {
            get { return imageData; }
            set
            {
                imageData = value; RaisePropertyChanged();
                hasImage = !string.IsNullOrWhiteSpace(value); RaisePropertyChanged("HasImage");
            }
        }

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
            
        }
    }
}

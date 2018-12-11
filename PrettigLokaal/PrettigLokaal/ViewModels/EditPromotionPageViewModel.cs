using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class EditPromotionPageViewModel : ViewModelBase
    {
        private string name = "";
        private DateTime startDate = DateTime.Now;
        private DateTime endDate = DateTime.Now;
        private ImageData imageData = null;
        private bool hasCouponCode = false;
        private string description = "";

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }
        public DateTime StartDate { get { return startDate; } set { startDate = value; RaisePropertyChanged(); } }
        public DateTime EndDate { get { return endDate; } set { endDate = value; RaisePropertyChanged(); } }
        public ImageData ImageData { get { return imageData; } set { imageData = value; RaisePropertyChanged(); } }
        public bool HasCouponCode { get { return hasCouponCode; } set { hasCouponCode = value; RaisePropertyChanged(); } }
        public string Description { get { return description; } set { description = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            // TODO: Validate 
            if (string.IsNullOrWhiteSpace(Name))
                ModelErrors["Name"] = "Gelieve een geldige naam in te voeren.";

            if (EndDate.CompareTo(DateTime.Now) > 0)
                ModelErrors["EndDate"] = "Gelieve een datum na vandaag in te vullen.";

            if (EndDate.CompareTo(StartDate) > 0)
                ModelErrors["EndDate"] = "Gelieve een geldig start datum in te voeren.";

            if (ImageData == null)
                ModelErrors["Image"] = "Gelieve een geldige foto te kiezen.";

            if (string.IsNullOrWhiteSpace(Description))
                ModelErrors["Description"] = "Gelieve een geldige omschrijving in te voeren.";
        }
    }
}

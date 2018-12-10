using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class MerchantPanelViewModel : ViewModelBase
    {
        private List<Image> images;
        private List<Event> events;
        private List<Promotion> promotions;

        private bool addImageLoading = false;

        public List<Image> Images { get { return images; } set { images = value; RaisePropertyChanged(); } }
        public List<Event> Events { get { return events; } set { events = value; RaisePropertyChanged(); } }
        public List<Promotion> Promotions { get { return promotions; } set { promotions = value; RaisePropertyChanged(); } }

        public bool AddImageLoading { get { return addImageLoading; } set { addImageLoading = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            // Nothing to do    
        }
    }
}

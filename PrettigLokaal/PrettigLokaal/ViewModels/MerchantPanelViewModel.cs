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
        private bool addImageLoading = false;

        public List<Image> Images { get { return images; } set { images = value; RaisePropertyChanged(); } }

        public bool AddImageLoading { get { return addImageLoading; } set { addImageLoading = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            // Nothing to do    
        }
    }
}

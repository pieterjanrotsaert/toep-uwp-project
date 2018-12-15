using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class DiscoverPageViewModel : ViewModelBase
    {
        private List<Merchant> merchants;

        public List<Merchant> Merchants { get { return merchants; } set { merchants = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
            throw new NotImplementedException();
        }
    }
}

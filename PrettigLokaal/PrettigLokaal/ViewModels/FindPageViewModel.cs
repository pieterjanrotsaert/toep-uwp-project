using PrettigLokaal.ViewModels.Helpers;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class FindPageViewModel : ViewModelBase
    {
        private ObservableCollection<Merchant> merchants;
        private string searchQuery;
        private bool isLoading = true;
        private int resultCount = 0;

        public ObservableCollection<Merchant> Merchants { get { return merchants; } set { merchants = value; RaisePropertyChanged(); } }
        public string SearchQuery { get { return searchQuery; } set { searchQuery = value; RaisePropertyChanged(); } }
        public bool IsLoading { get { return isLoading; } set { isLoading = value; RaisePropertyChanged(); } }
        public int ResultCount { get { return resultCount; } set { resultCount = value; RaisePropertyChanged(); } }

        protected override void ValidateSelf()
        {
           
        }
    }
}

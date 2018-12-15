using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PrettigLokaal
{
    public sealed partial class DiscoverPage : Page
    {
        private DiscoverPageViewModel ViewModel;
        private MainPage mainPage;

        public DiscoverPage()
        {
            InitializeComponent();
            ViewModel = new DiscoverPageViewModel();
            DataContext = ViewModel;
            RefreshPage();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

            mainPage.EnsureNavItemSelected("nav_discover");
        }

        private void RefreshPage(bool showLoader = true)
        {
            //if (showLoader)
            //    mainPage.SetLoading(true);
            API.Get().GetFeaturedMerchants((merchants, err) =>
            {
                //if (showLoader)
                //    mainPage.SetLoading(false);

                if(err != null)
                    Utils.ErrorBox(err);
                else
                {
                    ViewModel.Merchants = merchants;
                }
            });
        }
    }
}
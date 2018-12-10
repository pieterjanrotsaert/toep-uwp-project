using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PrettigLokaal
{
    public sealed partial class FeedPage : Page
    {
        //private MainPageViewModel _viewModel;
        private MainPage mainPage;

        public FeedPage()
        {
            InitializeComponent();
            //_viewModel = new MainPageViewModel();
            //DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

            mainPage.EnsureNavItemSelected("nav_feed");
        }
    }

    
}
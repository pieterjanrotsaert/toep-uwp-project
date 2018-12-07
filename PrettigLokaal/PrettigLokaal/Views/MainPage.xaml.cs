using PrettigLokaal.Backend;
using PrettigLokaal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace PrettigLokaal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel();
            DataContext = _viewModel;

            API.Get().Init(err =>
            {
                // Navigate to appropriate page
            });
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            var item = FindNavItemByTag("nav_feed");
            if (item != null)
                NavView.SelectedItem = item;
        }

        private NavigationViewItem FindNavItemByTag(string tag)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "nav_feed")
                    return (NavigationViewItem)item;
            }
            return null;
        }

        private void PopulateMenu()
        {

        }

        private void SignoutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(API.Get().IsLoggedIn())
            {

            }
            else
            {

            }
            e.Handled = true;
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            if (item == null)
                return;

            switch(item.Tag.ToString())
            {
                case "nav_feed":
                    ContentFrame.Navigate(typeof(FeedPage), this);
                    break;
                case "nav_discover":
                    ContentFrame.Navigate(typeof(DiscoverPage), this);
                    break;
                case "nav_coupons":
                    ContentFrame.Navigate(typeof(MyCouponsPage), this);
                    break;
                case "nav_merchantpanel":
                    ContentFrame.Navigate(typeof(MerchantPanel), this);
                    break;
            }
        }
        
        
    }
}

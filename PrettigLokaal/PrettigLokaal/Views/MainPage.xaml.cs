using PrettigLokaal.Backend;
using PrettigLokaal.ViewModels;
using PrettigLokaal.Views;
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
        private MainPageViewModel viewModel;
        private string startPage = "";
        private bool navViewLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            viewModel = new MainPageViewModel();
            DataContext = viewModel;

            API.Get().Init(err =>
            {
                viewModel.IsLoggedIn = API.Get().IsLoggedIn();
                viewModel.IsMerchant = API.Get().IsMerchant();

                if(API.Get().IsLoggedIn())
                {
                    startPage = "nav_feed";
                }
                else
                {
                    startPage = "nav_discover";
                }

                if (navViewLoaded)
                    SelectNavItem(startPage);
            });
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            navViewLoaded = true;
            if (startPage.Length <= 0)
                return;
            SelectNavItem(startPage);
        }

        private void SelectNavItem(string tag)
        {
            var item = FindNavItemByTag(startPage);
            if (item != null)
                NavView.SelectedItem = item;
        }

        private NavigationViewItem FindNavItemByTag(string tag)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                    return (NavigationViewItem)item;
            }
            return null;
        }

        private void SignoutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(API.Get().IsLoggedIn())
            {
                SetLoading(true);
                API.Get().Logout(err => 
                {
                    SetLoading(false);
                    viewModel.IsMerchant = false;
                    viewModel.IsLoggedIn = false;
                });
            }
            e.Handled = true;
        }

        private void ClearNavSelection()
        {
            NavView.SelectedItem = NavView.SettingsItem; // Use the settings item to 'deselect' the current one, it's hidden anyway.
        }

        private void SigninButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Aanmelden";
            ContentFrame.Navigate(typeof(LoginPage), this);
        }

        private void RegisterButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Registreren";
            ContentFrame.Navigate(typeof(RegisterPage), this);
        }

        private void ChangePassword_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Wachtwoord wijzigen";
            ContentFrame.Navigate(typeof(ChangePasswordPage), this);
        }
    
        public void GoBack()
        {
            ContentFrame.GoBack();
        }

        public void GoHome()
        {
            SelectNavItem("nav_discover");
        }

        public void OnSignInComplete()
        {
            viewModel.IsLoggedIn = API.Get().IsLoggedIn();
            viewModel.IsMerchant = API.Get().IsMerchant();
        }

        public void SetLoading(bool state)
        {
            viewModel.IsLoading = state;
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            if (item == null)
                return;
            if (item.Tag == null)
                return;

            switch(item.Tag.ToString())
            {
                case "nav_feed":
                    viewModel.Title = "Feed";
                    ContentFrame.Navigate(typeof(FeedPage), this);
                    break;
                case "nav_discover":
                    viewModel.Title = "Ontdek";
                    ContentFrame.Navigate(typeof(DiscoverPage), this);
                    break;
                case "nav_coupons":
                    viewModel.Title = "Coupons";
                    ContentFrame.Navigate(typeof(MyCouponsPage), this);
                    break;
                case "nav_merchantpanel":
                    viewModel.Title = "Zaakbeheer";
                    ContentFrame.Navigate(typeof(MerchantPanel), this);
                    break;
            }
        }

        
    }
}

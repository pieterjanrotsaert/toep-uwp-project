using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
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
                viewModel.ShowMerchantSignup = viewModel.IsLoggedIn && !viewModel.IsMerchant;

                if (API.Get().IsLoggedIn())
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

        // Navigate to an item in the NavView
        public void SelectNavItem(string tag)
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
            if (API.Get().IsLoggedIn())
            {
                Utils.YesNoPrompt("Bent u zeker dat u wilt afmelden?\n" + 
                    "Sommige features zijn niet beschikbaar wanneer je niet bent aangemeld.", "Afmelden", () =>
                {
                    SetLoading(true);
                    API.Get().Logout(err =>
                    {
                        SetLoading(false);
                        viewModel.IsMerchant = false;
                        viewModel.IsLoggedIn = false;
                        viewModel.ShowMerchantSignup = false;
                    });
                });
            }
            
            e.Handled = true;
        }

        private void ClearNavSelection()
        {
            // Use the built-in settings item to 'deselect' the current one, it's hidden anyway.
            NavView.SelectedItem = NavView.SettingsItem; 
        }

        public void EnsureNavItemSelected(string tag)
        {
            var item = (NavigationViewItem)NavView.SelectedItem;
            if (item != null)
            {
                if (item.Visibility == Visibility.Collapsed)
                    return;

                if ((string)item.Tag != tag)
                    SelectNavItem(tag);
            }
            else
                SelectNavItem(tag);
        }

        private void SigninButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Aanmelden";
            ContentFrame.Navigate(typeof(LoginPage), this);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        private void RegisterButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Registreren";
            ContentFrame.Navigate(typeof(RegisterPage), this);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        private void ChangePassword_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ClearNavSelection();
            viewModel.Title = "Wachtwoord wijzigen";
            ContentFrame.Navigate(typeof(ChangePasswordPage), this);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        public void NavigateToPage(Type pageType, object pageParams, string title)
        {
            ClearNavSelection();
            viewModel.Title = title;
            ContentFrame.Navigate(pageType, pageParams);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }
    
        public void GoBack()
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

        public void GoHome()
        {
            // TODO: Determine proper home item based on loginstatus
            SelectNavItem("nav_discover");
        }

        public void OnSignInStatusChanged()
        {
            viewModel.IsLoggedIn = API.Get().IsLoggedIn();
            viewModel.IsMerchant = API.Get().IsMerchant();
            viewModel.ShowMerchantSignup = viewModel.IsLoggedIn && !viewModel.IsMerchant;
        }

        public void OnMerchantSignInStatusChanged()
        {
            viewModel.IsMerchant = API.Get().IsMerchant();
            viewModel.ShowMerchantSignup = API.Get().IsLoggedIn() && !viewModel.IsMerchant;
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
                case "nav_merchantsettings":
                    viewModel.Title = "Zaakdetails Wijzigen";
                    ContentFrame.Navigate(typeof(MerchantRegisterPage), this);
                    break;
                case "nav_merchantregister":
                    viewModel.Title = "Registreren als Handelaar";
                    ContentFrame.Navigate(typeof(MerchantRegisterPage), this);
                    break;
            }
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
            viewModel.CanGoBack = ContentFrame.CanGoBack;
        }
    }
}

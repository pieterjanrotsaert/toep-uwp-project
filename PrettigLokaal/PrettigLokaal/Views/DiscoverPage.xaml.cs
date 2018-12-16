using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaal.Views;
using PrettigLokaalBackend.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Image = PrettigLokaalBackend.Models.Domain.Image;

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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

            mainPage.EnsureNavItemSelected("nav_discover");
            RefreshPage();
        }

        private void RefreshPage(bool showLoader = true)
        {
            if (showLoader)
                ViewModel.IsLoading = true;
            mainPage.SetLoading(true);
            API.Get().GetDiscover((model, err) =>
            {
                mainPage.SetLoading(false);
                ViewModel.IsLoading = false;
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    ViewModel.FeaturedMerchants = model.FeaturedMerchants;
                    GetImages(ViewModel.FeaturedMerchants);
                    ViewModel.RecentlyAddedMerchants = model.RecentlyAddedMerchants;
                    GetImages(ViewModel.RecentlyAddedMerchants);
                    ViewModel.Promotions = model.Promotions;
                    foreach (var pr in ViewModel.Promotions)
                    {
                        var evIndex = ViewModel.Promotions.IndexOf(pr);
                        if (pr.Image?.Data?.Data == null && pr.Image != null)
                            API.Get().GetImage(pr.Image.Id, (downloadedImage, err2) =>
                            {
                                if (err2 != null)
                                    return;

                                List<Promotion> newProms = new List<Promotion>();
                                foreach (var obj in ViewModel.Promotions)
                                    newProms.Add(obj);

                                downloadedImage.IsLoading = false;
                                newProms[evIndex] = new Promotion(pr) { Image = downloadedImage };
                                ViewModel.Promotions = newProms;
                                ViewModel.RaisePropertyChanged("Promotions");
                            });
                    }
                    ViewModel.Events = model.Events;
                    foreach (var ev in ViewModel.Events)
                    {
                        var evIndex = ViewModel.Events.IndexOf(ev);
                        if (ev.Image?.Data?.Data == null && ev.Image != null)
                            API.Get().GetImage(ev.Image.Id, (downloadedImage, err2) =>
                            {
                                if (err2 != null)
                                    return;

                                List<Event> newEvents = new List<Event>();
                                foreach (var obj in ViewModel.Events)
                                    newEvents.Add(obj);

                                downloadedImage.IsLoading = false;
                                newEvents[evIndex] = new Event(ev) { Image = downloadedImage };
                                ViewModel.Events = newEvents;
                                ViewModel.RaisePropertyChanged("Events");
                            });
                    }
                    ViewModel.EventPromotionMerchants = model.EventPromotionMerchants;
                }
            });
        }

        private void GetImages(List<Merchant> merchants)
        {
            foreach (var mc in merchants)
                if (mc.Images != null)
                    foreach (var img in mc.Images)
                    {
                        var imgIndex = mc.Images.IndexOf(img);
                        if (img.Data?.Data == null)
                            API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                            {
                                if (err2 != null)
                                    return;

                                List<Image> newImages = new List<Image>();
                                foreach (var image in mc.Images)
                                    newImages.Add(image);

                                downloadedImage.IsLoading = false;
                                newImages[imgIndex] = downloadedImage;
                                mc.Images = newImages;
                                mc.RaisePropertyChanged("Images");
                                mc.RaisePropertyChanged("Merchant");
                            });
                    }
        }

        private void ShowCouponButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void MerchantButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int id = (int)((Windows.UI.Xaml.Controls.Button)sender).Tag;
            var merchant = ViewModel.EventPromotionMerchants.Where(m => m.Id == id).FirstOrDefault();
            mainPage.NavigateToPage(typeof(MerchantPage),
                new MerchantPage.NavigationParams() { mainPage = mainPage, merchant = merchant },
                merchant.Name);
        }

        private void FeaturedMerchantClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int id = (int)((Windows.UI.Xaml.Controls.Grid)sender).Tag;
            var merchant = ViewModel.FeaturedMerchants.Where(m => m.Id == id).FirstOrDefault();
            mainPage.NavigateToPage(typeof(MerchantPage),
                new MerchantPage.NavigationParams() { mainPage = mainPage, merchant = merchant },
                merchant.Name);
        }

        private void RecentlyAddedMerchantClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int id = (int)((Windows.UI.Xaml.Controls.Grid)sender).Tag;
            var merchant = ViewModel.FeaturedMerchants.Where(m => m.Id == id).FirstOrDefault();
            mainPage.NavigateToPage(typeof(MerchantPage),
                new MerchantPage.NavigationParams() { mainPage = mainPage, merchant = merchant },
                merchant.Name);
        }
    }
}
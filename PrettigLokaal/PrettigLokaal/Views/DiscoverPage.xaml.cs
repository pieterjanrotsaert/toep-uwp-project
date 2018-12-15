using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Domain;
using System.Collections.Generic;
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
            API.Get().GetFeaturedMerchants((merchants, err) =>
            {
                ViewModel.IsLoading = false;
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    ViewModel.FeaturedMerchants = merchants;
                    foreach (var merchant in ViewModel.FeaturedMerchants)
                        if (merchant.Images != null)
                            foreach (var img in merchant.Images)
                            {
                                var imgIndex = merchant.Images.IndexOf(img);
                                if (img.Data?.Data == null)
                                    API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                                    {
                                        if (err2 != null)
                                            return;

                                        List<Image> newImages = new List<Image>();
                                        foreach (var image in merchant.Images)
                                            newImages.Add(image);

                                        downloadedImage.IsLoading = false;
                                        newImages[imgIndex] = downloadedImage;
                                        merchant.Images = newImages;
                                        merchant.RaisePropertyChanged("Images");
                                        merchant.RaisePropertyChanged("Merchant");
                                    });
                            }
                }
            });

            API.Get().GetRecentlyAddedMerchants((merchants, err) =>
            {
                ViewModel.IsLoading = false;
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    ViewModel.RecentlyAddedMerchants = merchants;
                    foreach (var merchant in ViewModel.RecentlyAddedMerchants)
                        if (merchant.Images != null)
                            foreach (var img in merchant.Images)
                            {
                                var imgIndex = merchant.Images.IndexOf(img);
                                if (img.Data?.Data == null)
                                    API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                                    {
                                        if (err2 != null)
                                            return;

                                        List<Image> newImages = new List<Image>();
                                        foreach (var image in merchant.Images)
                                            newImages.Add(image);

                                        downloadedImage.IsLoading = false;
                                        newImages[imgIndex] = downloadedImage;
                                        merchant.Images = newImages;
                                        merchant.RaisePropertyChanged("Images");
                                        merchant.RaisePropertyChanged("Merchant");
                                    });
                            }
                }
            });
        }
    }
}
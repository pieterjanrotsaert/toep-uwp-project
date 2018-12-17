using System;
using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaal.Views;
using PrettigLokaalBackend.Models.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml.Navigation;

namespace PrettigLokaal
{
    public sealed partial class FeedPage : Windows.UI.Xaml.Controls.Page
    {
        private FeedPageViewModel viewModel;
        private MainPage mainPage;

        public FeedPage()
        {
            InitializeComponent();
            viewModel = new FeedPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);
            mainPage.EnsureNavItemSelected("nav_feed");
            Refresh();
        }

        private void Refresh()
        {
            mainPage.SetLoading(true);
            API.Get().GetFeed((result, err) =>
            {
                mainPage.SetLoading(false);
                if (result != null)
                {
                    viewModel.FeaturedMerchants = result.FeaturedMerchants;
                    viewModel.FollowedMerchants = result.FollowedMerchants;
                    viewModel.Events = result.Events;
                    viewModel.Promotions = result.Promotions;

                    viewModel.NotFollowing = viewModel.FollowedMerchants.Count == 0;

                    

                    List<Merchant> merchants = new List<Merchant>();
                    foreach(var merch in viewModel.FeaturedMerchants)
                    {
                        var followedMerch = viewModel.FollowedMerchants.Where(m => m.Id == merch.Id).FirstOrDefault();
                        if (followedMerch != null)  // Check if the same merchant is in followed merchants, if so, delete it there and 
                                                    // replace it with the same instance from FeaturedMerchants so we don't have to fetch 
                                                    // the images twice.
                        {
                            viewModel.FollowedMerchants.Remove(followedMerch);
                            viewModel.FollowedMerchants.Add(merch);
                        }

                        if (!merchants.Any(m => m.Id == merch.Id))
                            merchants.Add(merch);
                    }
                    foreach (var merch in viewModel.FollowedMerchants)
                    {
                        if (!merchants.Any(m => m.Id == merch.Id))
                            merchants.Add(merch);
                    }

                    // Fetch general merchant images 
                    foreach (var merchant in merchants)
                    {
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
                                    
                                    viewModel.RaisePropertyChanged("FollowedMerchants");
                                    viewModel.RaisePropertyChanged("FeaturedMerchants");
                                });
                        }
                    }

                    // Fetch event images
                    foreach (var ev in viewModel.Events)
                    {
                        var evIndex = viewModel.Events.IndexOf(ev);
                        if (ev.Image?.Data?.Data == null && ev.Image != null)
                            API.Get().GetImage(ev.Image.Id, (downloadedImage, err2) =>
                            {
                                if (err2 != null)
                                    return;

                                List<Event> newEvents = new List<Event>();
                                foreach (var obj in viewModel.Events)
                                    newEvents.Add(obj);

                                downloadedImage.IsLoading = false;
                                newEvents[evIndex] = new Event(ev) { Image = downloadedImage };
                                viewModel.Events = newEvents;
                                viewModel.RaisePropertyChanged("Events");
                            });
                    }

                    // Fetch promotion images
                    foreach (var ev in viewModel.Promotions)
                    {
                        var evIndex = viewModel.Promotions.IndexOf(ev);
                        if (ev.Image?.Data?.Data == null && ev.Image != null)
                            API.Get().GetImage(ev.Image.Id, (downloadedImage, err2) =>
                            {
                                if (err2 != null)
                                    return;

                                List<Promotion> newProms = new List<Promotion>();
                                foreach (var obj in viewModel.Promotions)
                                    newProms.Add(obj);

                                downloadedImage.IsLoading = false;
                                newProms[evIndex] = new Promotion(ev) { Image = downloadedImage };
                                viewModel.Promotions = newProms;
                                viewModel.RaisePropertyChanged("Promotions");
                            });
                    }


                }
                else if (err != null)
                    Utils.ErrorBox(err);
            });
        }

        private async void ShowCouponButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Utils.InfoBox("Pdf wordt gegenereerd", "Pdf");
            Windows.System.LauncherOptions options = new Windows.System.LauncherOptions();
            options.ContentType = "application/pdf";
            await API.Get().downloadPdfAsync((int)((Windows.UI.Xaml.Controls.Button)sender).Tag, async (stream, err) =>
            {
                string fileName = "Coupon.pdf";
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile storageFile = await storageFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                using (Stream input = stream)
                {
                    var output = await storageFile.OpenStreamForWriteAsync();
                    await input.CopyToAsync(output);
                    output.Close();
                    input.Close();
                }
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            });
        }

        private void MerchantButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int id = (int)((Windows.UI.Xaml.Controls.Button)sender).Tag;
            var merchant = viewModel.FollowedMerchants.Where(m => m.Id == id).FirstOrDefault();
            mainPage.NavigateToPage(typeof(MerchantPage),
                new MerchantPage.NavigationParams() { mainPage = mainPage, merchant = merchant },
                merchant.Name);
        }

    }

    
}
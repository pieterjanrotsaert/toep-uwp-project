using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PrettigLokaal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MerchantPage : Windows.UI.Xaml.Controls.Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public Merchant merchant;
        }

        private MerchantPageViewModel viewModel;
        private MainPage mainPage;

        public MerchantPage()
        {
            InitializeComponent();
            viewModel = new MerchantPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationParams args = (NavigationParams)e.Parameter;
            viewModel.Merchant = args.merchant;
            mainPage = args.mainPage;

            viewModel.TagLine = string.Join(", ", viewModel.Merchant.Tags.Select(t => t.Text));
            viewModel.HasFacebook = !string.IsNullOrWhiteSpace(viewModel.Merchant.FacebookPage);

            var dayStrings = viewModel.Merchant.OpeningHours
                .Select(span => span.OpenTime.ToShortTimeString() + " - " + span.CloseTime.ToShortTimeString()).ToList();

            viewModel.OpeningHours = "";

            mainPage.SetLoading(true);
            if(API.Get().IsLoggedIn())
                API.Get().IsSubscribed(viewModel.Merchant.Id, (result, err) =>
                {
                    mainPage.SetLoading(false);
                    if (err != null)
                        Utils.ErrorBox(err);
                    else
                        viewModel.IsFollowing = result.State;
                });

            if(dayStrings.Count >= 7)
            {
                string str = "";
                str += "Maandag: ".PadRight(20) + dayStrings[0] + "\n";
                str += "Dinsdag: ".PadRight(20) + dayStrings[1] + "\n";
                str += "Woensdag: ".PadRight(20) + dayStrings[2] + "\n";
                str += "Donderdag: ".PadRight(20) + dayStrings[3] + "\n";
                str += "Vrijdag: ".PadRight(20) + dayStrings[4] + "\n";
                str += "Zaterdag: ".PadRight(20) + dayStrings[5] + "\n";
                str += "Zondag: ".PadRight(20) + dayStrings[6] + "\n";
                viewModel.OpeningHours = str;
            }

            // Fetch general images 
            if (args.merchant.Images != null)
                foreach (var img in viewModel.Merchant.Images)
                {
                    var imgIndex = args.merchant.Images.IndexOf(img);
                    if(img.Data?.Data == null)
                        API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                        {
                            if (err2 != null)
                                return;

                            List<Image> newImages = new List<Image>();
                            foreach (var image in args.merchant.Images)
                                newImages.Add(image);

                            downloadedImage.IsLoading = false;
                            newImages[imgIndex] = downloadedImage;
                            viewModel.Merchant.Images = newImages;
                            viewModel.Merchant.RaisePropertyChanged("Images");
                            viewModel.RaisePropertyChanged("Merchant");
                        });
                }

            // Fetch event images 
            if (args.merchant.Events != null)
                foreach (var ev in viewModel.Merchant.Events)
                {
                    var evIndex = args.merchant.Events.IndexOf(ev);
                    if (ev.Image?.Data?.Data == null && ev.Image != null)
                        API.Get().GetImage(ev.Image.Id, (downloadedImage, err2) =>
                        {
                            if (err2 != null)
                                return;

                            List<Event> newEvents = new List<Event>();
                            foreach (var obj in args.merchant.Events)
                                newEvents.Add(obj);

                            downloadedImage.IsLoading = false;
                            newEvents[evIndex] = new Event(ev) { Image = downloadedImage };
                            viewModel.Merchant.Events = newEvents;
                            viewModel.Merchant.RaisePropertyChanged("Events");
                            viewModel.RaisePropertyChanged("Merchant");
                        });
                }

            // Fetch promotion images 
            if (args.merchant.Promotions != null)
                foreach (var prom in viewModel.Merchant.Promotions)
                {
                    var promIndex = args.merchant.Promotions.IndexOf(prom);
                    if (prom.Image?.Data?.Data == null && prom.Image != null)
                        API.Get().GetImage(prom.Image.Id, (downloadedImage, err2) =>
                        {
                            if (err2 != null)
                                return;

                            List<Promotion> newPromotions = new List<Promotion>();
                            foreach (var obj in args.merchant.Promotions)
                                newPromotions.Add(obj);

                            downloadedImage.IsLoading = false;
                            newPromotions[promIndex] = new Promotion(prom) { Image = downloadedImage };
                            viewModel.Merchant.Promotions = newPromotions;
                            viewModel.Merchant.RaisePropertyChanged("Promotions");
                            viewModel.RaisePropertyChanged("Merchant");
                        });
                }


        }

        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsFollowing = true;
            API.Get().Subscribe(viewModel.Merchant.Id, (err) =>
            {
                if (err != null)
                    Utils.ErrorBox(err);
            });
        }

        private void UnfollowButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsFollowing = false;
            API.Get().Unsubscribe(viewModel.Merchant.Id, (err) =>
            {
                if (err != null)
                    Utils.ErrorBox(err);
            });
        }

        private void ShowCouponButton_Click(object sender, RoutedEventArgs e)
        {
            int promotionId = (int)((Windows.UI.Xaml.Controls.Button)sender).Tag;

            // TODO: Coupon weergeven (in pdf?)
        }
    }
}

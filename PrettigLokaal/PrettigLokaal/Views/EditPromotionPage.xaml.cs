using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Domain;
using PrettigLokaalBackend.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Image = PrettigLokaalBackend.Models.Domain.Image;

namespace PrettigLokaal.Views
{
    public sealed partial class EditPromotionPage : Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public Promotion existingPromotion;
        }

        MainPage mainPage;
        Promotion existingPromotion;
        EditPromotionPageViewModel viewModel;

        public EditPromotionPage()
        {
            InitializeComponent();
            viewModel = new EditPromotionPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationParams args = (NavigationParams)e.Parameter;
            mainPage = args.mainPage;
            existingPromotion = args.existingPromotion; // Null if creating a new promotion, not-null if editing an existing promotion

            // TODO: Check if we're editing an existing promotion and initialize the view model if so.
            if(existingPromotion != null)
            {
                viewModel.Name = existingPromotion.Name;
                viewModel.StartDate = existingPromotion.StartDate;
                viewModel.EndDate = existingPromotion.EndDate;
                viewModel.ImageData = existingPromotion.ImageData;
                viewModel.ImageSelected = viewModel.ImageData != null ? true : false;
                viewModel.HasCouponCode = existingPromotion.HasCouponCode;
                viewModel.Description = existingPromotion.Description;
            }

            base.OnNavigatedTo(e);
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.PickImageFile(async file =>
            {
                try
                {
                    var stream = await file.OpenStreamForReadAsync();
                    byte[] buf = new byte[stream.Length];
                    await stream.ReadAsync(buf, 0, (int)stream.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    ImageData imgData = new ImageData();
                    imgData.Data = Convert.ToBase64String(buf);
                    viewModel.ImageData = imgData;
                    viewModel.ImageSelected = true;
                }
                catch (Exception ex)
                {
                    Utils.InfoBox("Er is een stront opgetreden: " + ex.Message, "Fout");
                }
            });
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ImageData = null;
            viewModel.ImageSelected = false;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;
            Image img = new Image();
            img.Data = viewModel.ImageData;
            img.Merchant = API.Get().GetAccountInfo().Merchant;
            MerchantAddPromotionModel promotionModel = new MerchantAddPromotionModel()
            {
                Name = viewModel.Name,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Description = viewModel.Description,
                HasCouponCode = viewModel.HasCouponCode,
                Image = img
            };
            
            if (existingPromotion != null)
            {
                promotionModel.Id = existingPromotion.Id;
                API.Get().UpdatePromotion(promotionModel, err =>
                {
                    mainPage.SetLoading(true);
                    if (err == null)
                        Utils.InfoBox("Uw promotie werd succesvol upgedate", "Promotie updated.");
                    else
                        Utils.InfoBox("Er is een fout opgetreden:" + err.GetDescription(), "Fout");
                    mainPage.GoBack();
                });
            }
            else
            {
                API.Get().AddPromotion(promotionModel, err =>
                {
                    mainPage.SetLoading(true);
                    if (err == null)
                        Utils.InfoBox("Uw promotie werd succesvol aangemaakt", "Promotie aangemaakt.");
                    else
                        Utils.InfoBox("Er is een fout opgetreden:" + err.GetDescription(), "Fout");
                    mainPage.GoBack();
                });
            }
        }
    }
}

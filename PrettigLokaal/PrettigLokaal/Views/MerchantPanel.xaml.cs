using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PrettigLokaal.Views
{
    public sealed partial class MerchantPanel : Page
    {
        private MerchantPanelViewModel viewModel;
        private MainPage mainPage;

        public MerchantPanel()
        {
            InitializeComponent();
            viewModel = new MerchantPanelViewModel();
            DataContext = viewModel;
        }

        private void RefreshPage(bool showLoader = true)
        {
            if(showLoader)
                mainPage.SetLoading(true);
            API.Get().GetAccountMerchantData((merchant, err) =>
            {
                if(showLoader)
                    mainPage.SetLoading(false);

                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    viewModel.Promotions = merchant.Promotions;
                    viewModel.Events = merchant.Events;

                    // Download general images
                    viewModel.Images = merchant.Images;
                    foreach(var img in viewModel.Images)
                    {
                        API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                        {
                            if(err2 == null)
                                img.Data = downloadedImage.Data;
                                
                            viewModel.Images = viewModel.Images

                            .Select(m =>  m.Id == img.Id ? new PrettigLokaalBackend.Models.Domain.Image(img.Id, img.Data.Data, false) : m.Clone())
                            .ToList();

                        });
                    }

                    // Download event images
                    foreach (var ev in viewModel.Events)
                    {
                        API.Get().GetImage(ev.Image.Id, (downloadedImage, err2) =>
                        {
                            if (err2 == null)
                                ev.ImageData = downloadedImage.Data;

                            viewModel.Events = viewModel.Events
                            .Select(m => m.Id == ev.Id ? new Event(ev) { ImageDataLoading = false } : m.Clone())
                            .ToList();

                        });
                    }

                    // Download Promotion images
                    foreach (var prom in viewModel.Promotions)
                    {
                        API.Get().GetImage(prom.Image.Id, (downloadedImage, err2) =>
                        {
                            if (err2 == null)
                                prom.ImageData = downloadedImage.Data;

                            viewModel.Promotions = viewModel.Promotions
                            .Select(m => m.Id == prom.Id ? new Promotion(prom) { ImageDataLoading = false } : m.Clone())
                            .ToList();
                        });
                    }
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

            mainPage.EnsureNavItemSelected("nav_merchantpanel");

            RefreshPage();
        }

        private void AddImagesButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.PickImageFiles(async files =>
            {
                viewModel.AddImageLoading = true;
                List<string> fileData = new List<string>();
                foreach (StorageFile file in files)
                {
                    try
                    {
                        var stream = await file.OpenStreamForReadAsync();
                        byte[] buf = new byte[stream.Length];
                        await stream.ReadAsync(buf, 0, (int)stream.Length);
                        stream.Seek(0, SeekOrigin.Begin);

                        fileData.Add(Convert.ToBase64String(buf));
                    }
                    catch (Exception ex)
                    {
                        Utils.InfoBox("Er is een fout opgetreden: " + ex.Message, "Fout");
                    }
                }

                if (files.Count <= 0)
                    viewModel.AddImageLoading = false;
                else
                {
                    API.Get().UploadMerchantImages(fileData, err =>
                    {
                        viewModel.AddImageLoading = false;
                        if (err != null)
                            Utils.ErrorBox(err);
                        else
                            RefreshPage();
                    });
                }

            });
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;

            API.Get().RemoveImage(id, err =>
            {
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    foreach(var img in viewModel.Images)
                    {
                        if(img.Id == id)
                        {
                            viewModel.Images.Remove(img);
                            viewModel.Images = viewModel.Images.Select(i => i.Clone()).ToList();
                            break;
                        }
                    }
                }
            });
        }

        private void AddPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            mainPage.NavigateToPage(typeof(EditPromotionPage), 
                new EditPromotionPage.NavigationParams { mainPage = mainPage }, "Promotie Toevoegen");

        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            mainPage.NavigateToPage(typeof(EditEventPage),
                new EditEventPage.NavigationParams { mainPage = mainPage }, "Evenement Toevoegen");
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            API.Get().RemoveEvent(id, err =>
            {
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    foreach (var ev in viewModel.Events)
                    {
                        if (ev.Id == id)
                        {
                            viewModel.Events.Remove(ev);
                            viewModel.Events = viewModel.Events.Select(i => i.Clone()).ToList();
                            break;
                        }
                    }
                }
            });
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;

            Event ev = viewModel.Events.Where(p => p.Id == id).FirstOrDefault();

            mainPage.NavigateToPage(typeof(EditEventPage),
                new EditEventPage.NavigationParams { mainPage = mainPage, existingEvent = ev }, "Evenement Wijzigen");
        }

        private void DeletePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            API.Get().RemovePromotion(id, err =>
            {
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
                    foreach (var ev in viewModel.Promotions)
                    {
                        if (ev.Id == id)
                        {
                            viewModel.Promotions.Remove(ev);
                            viewModel.Promotions = viewModel.Promotions.Select(i => i.Clone()).ToList();
                            break;
                        }
                    }
                }
            });
        }

        private void EditPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;

            Promotion prom = viewModel.Promotions.Where(p => p.Id == id).FirstOrDefault();

            mainPage.NavigateToPage(typeof(EditPromotionPage),
                new EditPromotionPage.NavigationParams { mainPage = mainPage, existingPromotion = prom }, "Promotie Wijzigen");
        }

    }
}
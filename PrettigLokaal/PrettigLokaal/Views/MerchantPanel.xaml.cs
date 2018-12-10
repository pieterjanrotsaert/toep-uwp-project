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

        private void RefreshPage()
        {
            mainPage.SetLoading(true);
            API.Get().GetAccountMerchantData((merchant, err) =>
            {
                mainPage.SetLoading(false);

                if (err != null)
                    Utils.ErrorBox(err);
                else
                {
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
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

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

        }
    }
}
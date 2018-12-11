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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace PrettigLokaal.Views
{
    public sealed partial class EditPromotionPage : Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public Promotion existingEvent;
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
            existingPromotion = args.existingEvent; // Null if creating a new promotion, not-null if editing an existing promotion

            // TODO: Check if we're editing an existing promotion and initialize the view model if so.

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
    }
}

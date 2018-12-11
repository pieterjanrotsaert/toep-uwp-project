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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PrettigLokaal.Views
{
    public sealed partial class EditEventPage : Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public Event existingEvent;
        }

        MainPage mainPage;
        Event existingEvent;
        EditEventPageViewModel viewModel;

        public EditEventPage()
        {
            InitializeComponent();
            viewModel = new EditEventPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationParams args  = (NavigationParams)e.Parameter;
            mainPage = args.mainPage;
            existingEvent = args.existingEvent; // Null if creating a new event, not-null if editing an existing event

            // TODO: Check if we're editing an existing event and initialize the view model if so.
            if (existingEvent != null)
            {
                viewModel.TitleString = "Event Aanpassen";
                viewModel.Name = existingEvent.Name;
                viewModel.StartDate = existingEvent.StartDate;
                viewModel.EndDate = existingEvent.EndDate;
                viewModel.ImageData = existingEvent.ImageData;
                viewModel.ImageSelected = viewModel.ImageData != null ? true : false;
                viewModel.Description = existingEvent.Description;
                viewModel.PlaceDescription = existingEvent.PlaceDescription;
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
                    Utils.InfoBox("Er is een fout opgetreden: " + ex.Message, "Fout");
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
            MerchantAddEventModel eventModel = new MerchantAddEventModel()
            {
                Name = viewModel.Name,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Description = viewModel.Description,
                PlaceDescription = viewModel.PlaceDescription,
                Image = img
            };

            mainPage.SetLoading(true);
            if (existingEvent != null)
            {
                eventModel.Id = existingEvent.Id;
                API.Get().UpdateEvent(eventModel, err =>
                {
                    if (err == null)
                        Utils.InfoBox("Uw promotie werd succesvol upgedate", "Promotie updated.");
                    else
                        Utils.InfoBox("Er is een fout opgetreden:" + err.GetDescription(), "Fout");
                    mainPage.GoBack();
                });
            }
            else
            {
                API.Get().AddEvent(eventModel, err =>
                {
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

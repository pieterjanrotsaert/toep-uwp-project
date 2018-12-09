using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class MerchantRegisterPage : Page
    {
        private MerchantRegisterPageViewModel viewModel;
        private MainPage mainPage;

        public MerchantRegisterPage()
        {
            InitializeComponent();
            viewModel = new MerchantRegisterPageViewModel();
            DataContext = viewModel;

            viewModel.IsMerchant = API.Get().IsMerchant();
            if(viewModel.IsMerchant)
            {

            }
            else
            {

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;

            if (!API.Get().IsMerchant())
            {
                mainPage.SetLoading(true);
                API.Get().RegisterAsMerchant(null, err =>
                {
                    mainPage.SetLoading(false);
                    if (err != null)
                        Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
                    else
                    {
                        Utils.InfoBox("De registratie was succesvol. U bent nu een handelaar en hebt toegang tot extra functionaliteiten.", "Registratie succesvol!");
                        mainPage.OnMerchantSignInComplete();
                    }
                });
            }
            else
            {
                mainPage.SetLoading(true);
                API.Get().UpdateMerchantDetails(null, err =>
                {
                    mainPage.SetLoading(false);
                    if (err != null)
                        Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
                    else
                    {
                        Utils.InfoBox("Uw gegevens zijn met succes gewijzigd.", "Wijziging succesvol");
                        mainPage.OnMerchantSignInComplete();
                    }
                });
            }
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.PickImageFile(async file =>
            {
                viewModel.SelectedFile = file;
                viewModel.SelectedImageFile = file.Path;
                try
                {
                    var stream = await file.OpenStreamForReadAsync();
                    byte[] buf = new byte[stream.Length];
                    await stream.ReadAsync(buf, 0, (int)stream.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    viewModel.ImageData = Convert.ToBase64String(buf);
                }
                catch(Exception ex)
                {
                    Utils.InfoBox("Er is een fout opgetreden: " + ex.Message, "Fout");
                }
            });
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.YesNoPrompt(
                "WAARSCHUWING: Uw handelaarsaccount wordt permanent verwijderd als u doorgaat.\nU verliest mogelijks uw gegevens.\n" + 
                "Wenst u toch door te gaan?",
                "Account Verwijderen", () =>
                {
                    API.Get().TerminateMerchantAccount(err =>
                    {
                        if (err == null)
                        {
                            mainPage.OnMerchantSignInComplete();
                            Utils.InfoBox("Uw account werd succesvol verwijderd.", "Account verwijderd.");
                        }
                        else
                            Utils.InfoBox("Er is een fout opgetreden:" + err.GetDescription(), "Fout");
                    });
                });
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ImageData = null;
            viewModel.SelectedImageFile = "Geen bestand geselecteerd.";
        }
    }
}

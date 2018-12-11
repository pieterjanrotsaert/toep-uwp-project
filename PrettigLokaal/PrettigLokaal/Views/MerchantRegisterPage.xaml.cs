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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);

            viewModel.IsMerchant = API.Get().IsMerchant();
            if (viewModel.IsMerchant)
            {
                mainPage.EnsureNavItemSelected("nav_merchantsettings");

                Merchant m = API.Get().GetAccountInfo().Merchant;
                viewModel.Address = m.Address;
                viewModel.ContactEmail = m.ContactEmail;
                viewModel.PhoneNumber = m.PhoneNumber;
                viewModel.TagList = string.Join(", ", m.Tags.Select(t => t.Text));
                viewModel.Description = m.Description;
                viewModel.FacebookLink = m.FacebookPage;
                viewModel.Name = m.Name;

                List<OpeningHourSpan> hrs = m.OpeningHours;

                viewModel.OpenTimeMonday = hrs[0].OpenTime;
                viewModel.OpenTimeTuesday = hrs[1].OpenTime;
                viewModel.OpenTimeWednesday = hrs[2].OpenTime;
                viewModel.OpenTimeThursday = hrs[3].OpenTime;
                viewModel.OpenTimeFriday = hrs[4].OpenTime;
                viewModel.OpenTimeSaturday = hrs[5].OpenTime;
                viewModel.OpenTimeSunday = hrs[6].OpenTime;

                viewModel.CloseTimeMonday = hrs[0].CloseTime;
                viewModel.CloseTimeTuesday = hrs[1].CloseTime;
                viewModel.CloseTimeWednesday = hrs[2].CloseTime;
                viewModel.CloseTimeThursday = hrs[3].CloseTime;
                viewModel.CloseTimeFriday = hrs[4].CloseTime;
                viewModel.CloseTimeSaturday = hrs[5].CloseTime;
                viewModel.CloseTimeSunday = hrs[6].CloseTime;
            }
            else
                mainPage.EnsureNavItemSelected("nav_merchantregister");

        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;

            List<string> tagList = viewModel.TagList.Split(',').Select(tag => tag.Trim()).ToList();

            MerchantRegisterModel model = new MerchantRegisterModel()
            {
                Name = viewModel.Name,
                Address = viewModel.Address,
                ContactEmail = viewModel.ContactEmail,
                PhoneNumber = viewModel.PhoneNumber,
                FacebookPage = viewModel.FacebookLink,
                Description = viewModel.Description,

                OpenTimeMonday = viewModel.OpenTimeMonday,
                OpenTimeTuesday = viewModel.OpenTimeTuesday,
                OpenTimeWednesday = viewModel.OpenTimeWednesday,
                OpenTimeThursday = viewModel.OpenTimeThursday,
                OpenTimeFriday = viewModel.OpenTimeFriday,
                OpenTimeSaturday = viewModel.OpenTimeSaturday,
                OpenTimeSunday = viewModel.OpenTimeSunday,

                CloseTimeMonday = viewModel.CloseTimeMonday,
                CloseTimeTuesday = viewModel.CloseTimeTuesday,
                CloseTimeWednesday = viewModel.CloseTimeWednesday,
                CloseTimeThursday = viewModel.CloseTimeThursday,
                CloseTimeFriday = viewModel.CloseTimeFriday,
                CloseTimeSaturday = viewModel.CloseTimeSaturday,
                CloseTimeSunday = viewModel.CloseTimeSunday,

                Tags = tagList
            };

            if (!API.Get().IsMerchant())
            {
                mainPage.SetLoading(true);
                API.Get().RegisterAsMerchant(model, err =>
                {
                    mainPage.SetLoading(false);
                    if (err != null)
                        Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
                    else
                    {
                        //Utils.InfoBox("De registratie was succesvol. U bent nu een handelaar en hebt toegang tot extra functionaliteiten.", "Registratie succesvol!");
                        mainPage.OnMerchantSignInStatusChanged();
                    }
                });
            }
            else
            {
                mainPage.SetLoading(true);
                API.Get().UpdateMerchantDetails(model, err =>
                {
                    mainPage.SetLoading(false);
                    if (err != null)
                        Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
                    else
                    {
                        //Utils.InfoBox("Uw gegevens zijn met succes gewijzigd.", "Wijziging succesvol");
                        mainPage.OnMerchantSignInStatusChanged();
                    }
                });
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.YesNoPrompt(
                "WAARSCHUWING: Uw handelaarsaccount wordt permanent verwijderd als u doorgaat.\nU verliest mogelijks uw gegevens.\n" + 
                "Wenst u toch door te gaan?",
                "Account Verwijderen", () =>
                {
                    mainPage.SetLoading(true);
                    API.Get().TerminateMerchantAccount(err =>
                    {
                        mainPage.SetLoading(false);
                        if (err == null)
                        {
                            mainPage.OnMerchantSignInStatusChanged();
                            Utils.InfoBox("Uw account werd succesvol verwijderd.", "Account verwijderd.");
                        }
                        else
                            Utils.InfoBox("Er is een fout opgetreden:" + err.GetDescription(), "Fout");
                    });
                });
        }

        /*private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
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
                catch (Exception ex)
                {
                    Utils.InfoBox("Er is een fout opgetreden: " + ex.Message, "Fout");
                }
            });
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ImageData = null;
            viewModel.SelectedImageFile = "Geen bestand geselecteerd.";
        }*/

    }
}

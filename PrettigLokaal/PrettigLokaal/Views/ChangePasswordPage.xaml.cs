using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PrettigLokaal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangePasswordPage : Page
    {
        private ChangePasswordPageViewModel viewModel;
        private MainPage mainPage;

        public ChangePasswordPage()
        {
            this.InitializeComponent();
            viewModel = new ChangePasswordPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;

            mainPage.SetLoading(true);
            API.Get().ChangePassword(viewModel.OldPassword, viewModel.Password, err =>
            {
                mainPage.SetLoading(false);

                if (err == null)
                    Utils.InfoBox("Uw wachtwoord werd met succes gewijzigd!", "Wachtwoord gewijzigd");
                else
                    Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
            });
        }
    }
}

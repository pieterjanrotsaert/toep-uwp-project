using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
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


namespace PrettigLokaal.Views
{

    public sealed partial class RegisterPage : Page
    {
        private RegisterPageViewModel viewModel;
        private MainPage mainPage;

        public RegisterPage()
        {
            InitializeComponent();
            viewModel = new RegisterPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;

            mainPage.SetLoading(true);
            API.Get().CreateAccount(viewModel.Email, viewModel.Password, viewModel.FullName, viewModel.BirthDate, err =>
            {
                mainPage.SetLoading(false);
                if (err != null)
                {
                    if (err.ErrorCode == ErrorModel.EMAIL_ALREADY_IN_USE)
                        viewModel.AddModelError("Email", err.GetDescription());
                    else
                        Utils.InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
                }
                else
                    mainPage.OnSignInStatusChanged();
            });
        }
    }
}

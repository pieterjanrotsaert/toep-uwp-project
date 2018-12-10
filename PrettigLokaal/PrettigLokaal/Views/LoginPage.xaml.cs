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
    public sealed partial class LoginPage : Page
    {
        private LoginPageViewModel viewModel;
        private MainPage mainPage;

        public LoginPage()
        {
            InitializeComponent();
            viewModel = new LoginPageViewModel();
            DataContext = viewModel;

            viewModel.Remember = API.Get().HasRememberedLoginInfo();
            if(viewModel.Remember)
            {
                viewModel.Email = API.Get().GetRememberedLoginName();
                viewModel.Password = API.Get().GetRememberedLoginPassword();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void BtnSignin_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Validate();
            if (!viewModel.IsValid)
                return;

            mainPage.SetLoading(true);

            if (viewModel.Remember)
                API.Get().RememberLoginInfo(viewModel.Email, viewModel.Password);
            else
                API.Get().ClearRememberedLoginInfo();

            API.Get().Login(viewModel.Email, viewModel.Password, err =>
            {
                mainPage.SetLoading(false);
                if (err != null)
                {
                    if (err.ErrorCode == ErrorModel.INVALID_USERNAME)
                        viewModel.AddModelError("Email", err.GetDescription());
                    else if (err.ErrorCode == ErrorModel.INVALID_PASSWORD)
                        viewModel.AddModelError("Password", err.GetDescription());
                    else
                        Utils.InfoBox(err.GetDescription(), "Fout");
                }
                else
                    mainPage.OnSignInStatusChanged();
            });
        }
    }
}

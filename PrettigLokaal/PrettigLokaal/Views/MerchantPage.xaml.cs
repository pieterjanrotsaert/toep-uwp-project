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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PrettigLokaal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MerchantPage : Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public Merchant merchant;
        }

        private Merchant viewModel;
        private MainPage mainPage;

        public MerchantPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationParams args = (NavigationParams)e.Parameter;
            viewModel = args.merchant;
            mainPage = args.mainPage;
            DataContext = mainPage;

        }


    }
}

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
            public Promotion existingPromotion;
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
            existingPromotion = args.existingPromotion; // Null if creating a new promotion, not-null if editing an existing promotion

            // TODO: Check if we're editing an existing promotion and initialize the view model if so.

            base.OnNavigatedTo(e);
        }
    }
}

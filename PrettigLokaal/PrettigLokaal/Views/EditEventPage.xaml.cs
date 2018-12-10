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

            base.OnNavigatedTo(e);
        }
    }
}

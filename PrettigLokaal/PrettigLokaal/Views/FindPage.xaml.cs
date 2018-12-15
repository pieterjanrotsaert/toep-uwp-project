using PrettigLokaal.Backend;
using PrettigLokaal.Misc;
using PrettigLokaal.ViewModels;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace PrettigLokaal.Views
{

    public sealed partial class FindPage : Windows.UI.Xaml.Controls.Page
    {
        public struct NavigationParams
        {
            public MainPage mainPage;
            public string searchQry;
        }

        NavigationParams navParams;
        MainPage mainPage;
        FindPageViewModel viewModel;

        public FindPage()
        {
            InitializeComponent();
            viewModel = new FindPageViewModel();
            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navParams = (NavigationParams)e.Parameter;
            mainPage = navParams.mainPage;
            viewModel.SearchQuery = navParams.searchQry;
            Refresh();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.Merchants = new ObservableCollection<Merchant>();
            API.Get().AbortRequests();
        }

        private void Refresh()
        {
            viewModel.IsLoading = true;
            viewModel.Merchants = new ObservableCollection<Merchant>();
            API.Get().FindMerchants(viewModel.SearchQuery, (merchants, err) =>
            {
                viewModel.IsLoading = false;
                if (err != null)
                    Utils.ErrorBox(err);
                else
                {

                    viewModel.ResultCount = merchants.Count;
                    foreach (var merchant in merchants)
                    {
                        viewModel.Merchants.Add(merchant);
                        if (merchant.Images != null)
                        {
                            foreach (var img in merchant.Images)
                            {
                                var imgIndex = merchant.Images.IndexOf(img);
                                API.Get().GetImage(img.Id, (downloadedImage, err2) =>
                                {
                                    if (err2 != null)
                                        return;

                                    List <Image> newImages = new List<Image>();
                                    foreach(var image in merchant.Images)
                                        newImages.Add(image);

                                    downloadedImage.IsLoading = false;
                                    newImages[imgIndex] = downloadedImage;
                                    merchant.Images = newImages;
                                    merchant.RaisePropertyChanged("Images");
                                    viewModel.RaisePropertyChanged("Merchants");
                                });
                            }
                        }
                    }
                }
            });
        }

        private void PageSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                viewModel.SearchQuery = ((Windows.UI.Xaml.Controls.TextBox)sender).Text;
                if(viewModel.SearchQuery.Trim().Length > 0)
                    Refresh();
                e.Handled = true;
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Windows.UI.Xaml.Controls.Button)sender).Tag;
            var merchant = viewModel.Merchants.Where(m => m.Id == id).FirstOrDefault();
            mainPage.NavigateToPage(typeof(MerchantPage), 
                new MerchantPage.NavigationParams() { mainPage = mainPage, merchant = merchant },
                merchant.Name);
        }
    }
}

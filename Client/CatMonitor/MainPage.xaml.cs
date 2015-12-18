using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatMonitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            refreshListview();
          
        }

        private async void refreshListview()
        {
            
            await App.container.CreateIfNotExistsAsync();
          
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;
            do
            {
                resultSegment = await App.container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 30, continuationToken, null, null);
                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);


            listView.ItemsSource = resultSegment.Results.Reverse();
        }

        private async void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as CloudBlockBlob;
                var blob = App.container.GetBlockBlobReference(item.Name);
                StorageFile file;

                   StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
                    file = await temporaryFolder.CreateFileAsync(item.Name,
                       CreationCollisionOption.ReplaceExisting);

                    var downloadTask = blob.DownloadToFileAsync(file);

                    await downloadTask;
                    image.Source = new BitmapImage(new Uri(file.Path));
                
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            refreshListview();
        }
    }
}

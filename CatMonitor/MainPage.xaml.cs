using GHIElectronics.UWP.Shields;
using Microsoft.Azure.Devices.Client;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatMonitor
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        MediaCapture MC;
        DispatcherTimer PictureTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
        DispatcherTimer TempTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        VisionServiceClient OxfordClient;
        FEZHAT Shield;
        DeviceClient IoTHub;
        bool SendTelemetry = true;
        bool DirectRecognition = false;

        CloudBlobContainer ImagesDir;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Init();
            Shield = await FEZHAT.CreateAsync();
            Shield.D2.Color = new FEZHAT.Color(255, 0, 0);    
        }

        private async Task Init()
        {
            
            MC = new MediaCapture();
            await MC.InitializeAsync();
            Realtime.Source = MC;
            await MC.StartPreviewAsync();
            if (DirectRecognition) OxfordClient = new VisionServiceClient("ce3d37851dd447698bd867471bd8c3c3");
            ImagesDir = await GetImagesBlobContainer();
            PictureTimer.Tick += TakePicture;
            PictureTimer.Start();
            TempTimer.Tick += MeasureData;
            TempTimer.Start();
        }

        private async Task<CloudBlobContainer> GetImagesBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=r2d2storage;AccountKey=UV+6L2Scr9nJAyaLp+jjZflRyr6K05guMafOFJQcZ85NMUOcA4oQmRFzmMR3djiV3gaYkr2z2rmC9Uol9dNPfg==;BlobEndpoint=https://r2d2storage.blob.core.windows.net/;TableEndpoint=https://r2d2storage.table.core.windows.net/;QueueEndpoint=https://r2d2storage.queue.core.windows.net/;FileEndpoint=https://r2d2storage.file.core.windows.net/");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("kitties");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions{PublicAccess = BlobContainerPublicAccessType.Blob});
            return container;
        }

        private async void MeasureData(object sender, object e)
        {
            var t = Shield.GetTemperature();
            var l = Shield.GetLightLevel();
            Telemetry.Text = $"Temp: {t}, Light: {l}";

        }

        private bool IsCatPresent(AnalysisResult res)
        {
            foreach(var c in res.Categories)
            {
                if (c.Name.ToLower().Contains("cat"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPersonPresent(AnalysisResult res)
        {
            foreach (var c in res.Categories)
            {
                if (c.Name.ToLower().Contains("people"))
                {
                    return true;
                }
            }
            return false;
        }


        private async void TakePicture(object sender, object e)
        {
            PictureTimer.Stop();
            var ms = new MemoryStream();
            await MC.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), ms.AsRandomAccessStream());
            BitmapImage bmp = new BitmapImage();
            ms.Position = 0;
            bmp.SetSource(ms.AsRandomAccessStream());
            Sample.Source = bmp;
            Info.Text = "Picture Taken, sending...";
            ms.Position = 0;
            await SendPicture(ms);
            Info.Text = "Done, sleeping...";

            /* This was used for local oxford recognition 
            ms.Position = 0;
            var res = await OxfordClient.AnalyzeImageAsync(ms);
            if (IsCatPresent(res))
            {
                Info.Text = "Cat is detected";
                Shield.D2.Color = new FEZHAT.Color(0, 255, 0);
            }
            else
            {
                if (IsPersonPresent(res))
                {
                    Info.Text = "Person is detected";
                    Shield.D2.Color = new FEZHAT.Color(0, 0, 255);
                }
                else
                {
                    Info.Text = "The is no Cat in sight";
                    Shield.D2.Color = new FEZHAT.Color(255, 0, 0);
                }
            }
            */
                            
            PictureTimer.Start();
        }

        private async Task SendPicture(MemoryStream ms)
        {
            var name = Guid.NewGuid().ToString();
            var b = ImagesDir.GetBlockBlobReference(name);
            b.Properties.ContentType = "image/jpeg";
            await b.UploadFromStreamAsync(ms.AsInputStream());
        }
    }
}

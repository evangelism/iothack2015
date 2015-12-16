using Microsoft.ProjectOxford.Vision;
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
        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        VisionServiceClient OxfordClient;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Init();
        }

        private async Task Init()
        {
            MC = new MediaCapture();
            await MC.InitializeAsync();
            Realtime.Source = MC;
            await MC.StartPreviewAsync();
            dt.Tick += TakePicture;
            dt.Start();
            OxfordClient = new VisionServiceClient("ce3d37851dd447698bd867471bd8c3c3");
        }

        private async void TakePicture(object sender, object e)
        {
            dt.Stop();
            var ms = new MemoryStream();
            await MC.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), ms.AsRandomAccessStream());
            BitmapImage bmp = new BitmapImage();
            ms.Position = 0;
            bmp.SetSource(ms.AsRandomAccessStream());
            Sample.Source = bmp;
            Info.Text = "Picture Taken, Recognizing...";
            ms.Position = 0;
            var res = await OxfordClient.AnalyzeImageAsync(ms);
            var txt = new StringBuilder();
            foreach (var c in res.Categories)
            {
                txt.Append(c.Name);
                txt.Append(" ");
            }
            Info.Text = txt.ToString();
            dt.Start();
        }
    }
}

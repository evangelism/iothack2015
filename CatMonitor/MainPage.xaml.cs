using GHIElectronics.UWP.Shields;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
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
        FEZHAT Shield;

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
            dt.Tick += TakePicture;
            dt.Start();
            OxfordClient = new VisionServiceClient("ce3d37851dd447698bd867471bd8c3c3");
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
            dt.Start();
        }
    }
}

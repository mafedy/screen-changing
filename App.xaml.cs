using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using PexelsDotNetSDK.Api;
using System.Drawing;
using System.IO;
using System.Net;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace ScreenChanging
{



    public partial class App : Application
    {
        private MyFunctions lambda = new MyFunctions();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();

            var DirPath = "C:/Users/Z0161359/source/repos/ScreenChanging/datas/wallpapers/";

            var AllFiles = Directory.EnumerateFiles(DirPath);

            //if (AllFiles.Any())
            //{
            //    lambda.LastImagesPaths = AllFiles.ToList();
            //    lambda.SetImagesOnGUI(win, DirPath);
            //}

            lambda.PexelsAPI(win);

        }
    }
;




    public partial class MyFunctions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public List<string> LastImagesPaths = new List<string> { "", "", "", ""};

        public string DirPath = "C:/Users/Z0161359/source/repos/ScreenChanging/datas/wallpapers/";


        public async void PexelsAPI(MainWindow mainWindow)
        {
            string queryWord = mainWindow.query.Text;
            //string queryWord ="desert";

            //pr.Progress = 0.2;
            //prog.Value = 20;

            var pexelsClient = new PexelsClient("563492ad6f917000010000014f768f9c9134428b89da41e3726dd566");

            var photosInformations = await pexelsClient.SearchPhotosAsync(queryWord, orientation:"landscape", size:"high", page:1, pageSize:4);

            IEnumerable<PexelsDotNetSDK.Models.Photo> photosInformationsList = photosInformations.photos.Take(4);

            var counter = 0;
            foreach (var photo in photosInformationsList)
            {
                var date = DateTime.Now.ToString("_MMddyyyy_HHmmss_");
                SaveImage(photo.source.original, Path.Combine(this.DirPath, photo.id + date + queryWord + ".jpeg"));
                LastImagesPaths[counter] = Path.Combine(this.DirPath, photo.id + date + queryWord + ".jpeg");
                counter++;

            }

            SetImagesOnGUI(mainWindow);
            DeleteUnusedImages();

        }


        public void SaveImage(string URL, string PathToSave)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(URL, PathToSave);
        }


        public void SetImagesOnGUI(MainWindow window)
        {
            //List<string> ImagesPaths = new List<string>{"1.jpeg", "2.jpeg", "3.jpeg", "4.jpeg"};
            List<System.Windows.Shapes.Rectangle> wpp = new List<System.Windows.Shapes.Rectangle> { window.WPP1, window.WPP2, window.WPP3, window.WPP4 };

            for(int i = 0; i < wpp.Count; i++)
            {
                if (File.Exists(LastImagesPaths[i]))
                {
                    wpp[i].Fill = new ImageBrush(new BitmapImage(new Uri(LastImagesPaths[i], UriKind.RelativeOrAbsolute)));
                }
                else
                {
                    wpp[i].Fill = new ImageBrush(new BitmapImage(new Uri("C:/Users/Z0161359/source/repos/ScreenChanging/datas/shutterstock_253521943.jpg", UriKind.RelativeOrAbsolute)));
                }
            }
        }


        public void DeleteUnusedImages()
        {
            var AllFiles = Directory.EnumerateFiles(this.DirPath);
            
            foreach (var file in AllFiles)
            {
                if (!LastImagesPaths.Contains(file))
                {
                    Trace.Write(file);
                    try
                    {
                        File.Delete(file);
                    }     
                    catch { }
                }
            }
        }

        public void SetWallpaper(bool IsChecked, string tempPath)
        {

            tempPath = CheckFilterSetting(IsChecked, tempPath);

            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public string CheckFilterSetting(bool IsChecked, string jpegFilePath)
        {
            string pngFilePath = "C:/Users/Z0161359/source/repos/ScreenChanging/datas/zf_autocruise_35.png";

            if (IsChecked)
            {
                using (Image backgroundImage = Image.FromFile(jpegFilePath))
                using (Image overlayImage = Image.FromFile(pngFilePath))
                {
                    // Création d'une nouvelle image de sortie avec les dimensions de l'image de fond
                    using (Bitmap mergedImage = new Bitmap(overlayImage.Width, overlayImage.Height))
                    {
                        // Création d'un objet Graphics à partir de l'image de sortie
                        using (Graphics graphics = Graphics.FromImage(mergedImage))
                        {
                            // Dessiner l'image de fond sur l'image de sortie
                            graphics.DrawImage(backgroundImage, 0, 0, overlayImage.Width, overlayImage.Height);

                            // Dessiner l'image avec transparence sur l'image de sortie
                            graphics.DrawImage(overlayImage, 0, 0, overlayImage.Width, overlayImage.Height);
                        }

                        // Enregistrer l'image de sortie fusionnée au format JPEG
                        string jpegFilePathWithFilter = "C:/Users/Z0161359/source/repos/ScreenChanging/datas/wallpapers/choosen_with_filter.jpeg";
                        mergedImage.Save(jpegFilePathWithFilter, ImageFormat.Jpeg);

                        return jpegFilePathWithFilter;
                    }
                }
            }

            return jpegFilePath;
        }
    }

    public class ProgressDestination
    {
        public double Progress { get; set; } // in range 0,1
        public string Description { get; set; } // Optional text 
    }

    public class ProgressPoller : INotifyPropertyChanged
    {
        public ProgressDestination Progress { get; } = new ProgressDestination();
        private readonly DispatcherTimer timer;
        public ProgressPoller()
        {
            timer = new DispatcherTimer();
            timer.Tick += OnTick;
            timer.Interval = TimeSpan.FromSeconds(0.2);
        }

        private void OnTick(object sender, EventArgs e)
        {
            Value = (int)(Progress.Progress * MaxValue);
            Description = Progress.Description;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Description));
        }

        public int Value { get; set; }
        public string Description { get; set; }
        public int MaxValue { get; } = 100;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

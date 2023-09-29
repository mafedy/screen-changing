using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenChanging
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ChoosenPath = "";
        public MyFunctions MF = new MyFunctions();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.StrokeThickness = 2;

            ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0);
            Rectangle rectangle = (Rectangle)sender;
            rectangle.RenderTransform = scaleTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = 1.1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);

        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.StrokeThickness = 0;

            ScaleTransform scaleTransform = new ScaleTransform(1.1, 1.1);
            Rectangle rectangle = (Rectangle)sender;
            rectangle.RenderTransform = scaleTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private void WPP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;

            ImageBrush imageBrush = rect.Fill as ImageBrush;
            ImageSource imageSource = imageBrush.ImageSource;

            if (imageSource is BitmapImage bitmapImage)
            {
                this.ChoosenPath = bitmapImage.UriSource.AbsolutePath;
            }

            WPP5.Fill = rect.Fill;
            ShowModalContent();
        }

        private void ShowModalContent()
        {
            ModalContainer.Visibility = Visibility.Visible;
        }

        private void HideModalContent()
        {
            WPP5.Fill = null;
            ModalContainer.Visibility = Visibility.Collapsed;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;

            RotateTransform rotateTransform = new RotateTransform(0){ CenterX = image.ActualHeight / 2, CenterY = image.ActualWidth / 2 }; ;
      
            image.RenderTransform = rotateTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = 90,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;

            RotateTransform rotateTransform = new RotateTransform(90) { CenterX = image.ActualHeight / 2, CenterY = image.ActualWidth / 2 }; ;

            image.RenderTransform = rotateTransform;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideModalContent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MF.SetWallpaper((bool)AutocruiseBox.IsChecked, this.ChoosenPath);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MF.PexelsAPI(this);
        }


        //private void query_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        // Enter key was pressed
        //        Console.WriteLine("Enter key was pressed");
        //        // Perform any actions you want to do when Enter is pressed
        //    }
        //}
    }
}

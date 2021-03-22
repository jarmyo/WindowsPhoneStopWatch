using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.System.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using basicStopWatch.Controles;


namespace basicStopWatch.Cronometro
{
    public sealed partial class PageCronometro
    {
        private readonly DispatcherTimer _dt = new DispatcherTimer();
        private readonly Stopwatch _relojVuelta = new Stopwatch();
        private readonly Stopwatch _relojPrincipal = new Stopwatch();
        private DisplayRequest _gDisplayRequest;
        private string _stReiniciar;
        private string _stVuelta;

        public PageCronometro()
        {
            InitializeComponent();
            
            Adjust1080();
            LoadString();

            NavigationCacheMode = NavigationCacheMode.Required;
            HMinute.Percentage = HSecond.Percentage =
                MinuteroSombra.Percentage = HTMinute.Percentage =
                HTSeconds.Percentage = 0.01;

            Grafico.Children.Clear();
          //  HMinute.SegmentColor = SegmentColor();
            
            _dt.Tick += TickerTick;
            _dt.Interval = new TimeSpan(0, 0, 0, 0, 1);

            ButtonReset.Text = _stReiniciar;
        }
        

        //private static SolidColorBrush SegmentColor()
        //{
        //    var c = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
        //    var secondColorFactor = SecondColorFactor;
        //    return new SolidColorBrush(
        //        Color.FromArgb(c.Color.A,
        //            (byte)(c.Color.R * secondColorFactor),
        //            (byte)(c.Color.G * secondColorFactor),
        //            (byte)(c.Color.B * secondColorFactor))
        //        ); 
        //}

        public void ShowRecordRing()
        {
            var t = (bool) _localSettings.Values["RecordRing"];
            HTMinute.Visibility = ETotal2.Visibility = ETotal.Visibility = HTSeconds.Visibility = t ? Visibility.Visible : Visibility.Collapsed;
          //  HMinute.SegmentColor = SegmentColor();
        }
        

        public void Adjust1080()
        {
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
         //   var scaleFactor = scaleFactor1.ToString();

            var height = ((int)(Window.Current.Bounds.Width * scaleFactor));
           

            //9 o 18

            const int ringrid = 280;
            const int hminute = 100;
            const int hsecond = 125;
            const int eMinute = 220;
            const int eSecond = 260;
            const double fonts = 29.33;
            var offset1 = 0;
         //   var offset2 = 0;

            switch (height)
            {
                case 1080:
                {
                    var s = scaleFactor - 2.2;
                     offset1 = 60 +  (200 * (int)s ); //40

                    //if (scaleFactor == 2.2)
                    //{
                    //    offset1 = 60;
                    //}
                    //if (scaleFactor == 2.4)
                    //{
                    //    offset1 = 100;
                    //}
                    break;
                }

                case 720:
                    {
                        var s = scaleFactor - 1.4;                        
                        offset1 = 100 - (200 * (int)s); //80

                        //if (scaleFactor == 1.8)
                        //{
                        //    offset1 = 20;
                        //}
                        //if (scaleFactor == 1.4)
                        //{
                        //    offset1 = 100;
                        //}
                        break;
                    }
            }

            RingGrid.Height = RingGrid.Width = ringrid + offset1; 
            HMinute.Radius = MinuteroSombra.Radius = hminute + (offset1/2); 
            HSecond.Radius = hsecond + (offset1/2); 
            EMinute.Height = EMinute.Width = eMinute + offset1;
            ESecond.Height = ESecond.Width = eSecond + offset1;
            ButtonReset.FontSize = fonts;
        }


        private void LoadString()
        {
            var loader = new ResourceLoader();
            _stReiniciar = loader.GetString(@"toggle1");
            _stVuelta = loader.GetString(@"toggle2");
        }

        private void Activate()
        {
            if (_gDisplayRequest == null)
            {
                _gDisplayRequest = new DisplayRequest();
            }


            if (_gDisplayRequest != null)
            {
                _gDisplayRequest.RequestActive();
            }
        }

        private void Release()
        {
            if (_gDisplayRequest != null)
            {
                _gDisplayRequest.RequestRelease();
            }
        }


        private void Compartir(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested +=
                    PrepararTextoCompartir;

                DataTransferManager.ShowShareUI();
            }
            catch (Exception es)
            {
                var mensaje = new MessageDialog(es.Message) { Title = "Error" };
                mensaje.ShowAsync();

            }
        }

        private void MostrarPromedio(object sender, RoutedEventArgs e)
        {
            try
            {                
                new MessageDialog(  Conf.TiempoFormato(
                    new TimeSpan(Convert.ToInt64(Tiempos.Children.Cast<LapControl>().Average(es => es.Ticks)))))
                    { Title = FlyMenu2.Text, Options = MessageDialogOptions.None }.ShowAsync();
                
            }
            catch (Exception es)
            {
                var mensaje = new MessageDialog(es.Message) {Title = "Error"};
                mensaje.ShowAsync();
                
            }
        }

        private void PrepararTextoCompartir(DataTransferManager sender, DataRequestedEventArgs e)
        {
            try
            {
                var request = e.Request;
                
                request.Data.Properties.Title = "basicStopwatch";
                request.Data.Properties.Description = "Laps";
                var vueltas = string.Empty;                
                foreach (var uiElement in Tiempos.Children)
                {
                    var v = (LapControl)uiElement;
                    vueltas = vueltas + v.Resultado + Environment.NewLine;
                
                }

                request.Data.SetText(vueltas);
                
            }
            catch (Exception es)
            {
                var mensaje = new MessageDialog(es.Message) { Title = "Sharing Error" };
                mensaje.ShowAsync();
            }
           
        }


    }
}
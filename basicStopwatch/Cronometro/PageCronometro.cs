using System;
using System.Linq;
using Windows.Phone.Devices.Notification;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using basicStopWatch.Controles;

namespace basicStopWatch.Cronometro
{
    public sealed partial class PageCronometro
    {
        private const int Alarma = 59;
        private const int TopGraph = 45;
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private readonly VibrationDevice _testVibrationDevice = VibrationDevice.GetDefault();
        private double _escala;
        private long _maximo;
        private long _minimo = long.MaxValue;
        private long _valortiempo;
        private const int MaxLaps = 365;
        private int _currentLaps;
        private const double Refact = 1.66;

        private void TickerTick(object sender, object e)
        {
            if (!_relojVuelta.IsRunning) return;

            var timerVuelta = _relojVuelta.Elapsed;
            var timerPrincipal = _relojPrincipal.Elapsed;

            TimerTextMain.Text = Conf.TiempoFormato(timerVuelta);
            TimerText.Text = Conf.TiempoFormato(timerPrincipal);

            HSecond.Percentage = PorcentajeSegundos(timerPrincipal);
            HTSeconds.Percentage = PorcentajeSegundos(timerVuelta);


            HMinute.Percentage = PorcentajeMinutos(timerPrincipal);
            HTMinute.Percentage = PorcentajeMinutos(timerVuelta);

            if (Conf.Sombra)
            {
                if ((timerVuelta.Minutes < Conf.ShadowTime) ||
                    (timerVuelta.Minutes ==(int)Conf.ShadowTime && timerVuelta.Seconds < 1))
                {
                    MinuteroSombra.Percentage = ((Conf.Div1 * timerVuelta.Minutes) + (Conf.Div2 * timerVuelta.Seconds));
                }
            }
            else
            {
                MinuteroSombra.Percentage = 0.01;
            }

            if (timerPrincipal.Seconds != Alarma) return;
            Vibrar();
            Sonar();
        }

        private static double PorcentajeSegundos(TimeSpan timerVuelta)
        {
            return (Refact * timerVuelta.Seconds) + (0.0016 * timerVuelta.Milliseconds); 
        }

        private static double PorcentajeMinutos(TimeSpan timerVuelta)
        {
            return (Refact * timerVuelta.Minutes) + (0.028 * timerVuelta.Seconds); 
        }

        private void Vibrar()
        {
            if (!Conf.Vibrar) return;
            try
            {
                _testVibrationDevice.Vibrate(TimeSpan.FromSeconds(1));
            }
            catch
            {
                Conf.Vibrar = false;
            }
        }

        private void Sonar()
        {
            if (!Conf.Sonar) return;
            try
            {
                Bleep.Play();
            }
            catch
            {
                Conf.Sonar = false;
            }
        }

        internal void Pausar()
        {
            ButtonPausar.Source = new BitmapImage
            {
                UriSource = new Uri("ms-appx:///Assets/play.png", UriKind.Absolute)
            };
            ButtonReset.Text = _stReiniciar;
            _relojVuelta.Stop();
            _relojPrincipal.Stop();
            Release();
        }

        internal void Continuar()
        {
            Activate();
            ButtonPausar.Source = new BitmapImage
            {
                UriSource = new Uri("ms-appx:///Assets/pause.png", UriKind.Absolute)
            };
            ButtonReset.Text = _stVuelta;
            _relojVuelta.Start();
            _relojPrincipal.Start();
            _dt.Start();
        }

        internal void AgregarVuelta()
        {
            if (!_relojVuelta.IsRunning) return;            
            if (_currentLaps >= MaxLaps) return;
          
                CalcularEscalas();

                _currentLaps++;

                Tiempos.Children.Insert(0, new LapControl
                {
                    Ticks = _relojPrincipal.Elapsed.Ticks,
                    Numero = _currentLaps,
                    Total = _relojVuelta.Elapsed.Ticks,
                    ValorTiempo = _valortiempo
                });

                _relojPrincipal.Reset();
                _relojPrincipal.Start();

                if ((bool) _localSettings.Values["ShowGraph"])
                {
                    var t = 1+ TopGraph - ((_maximo - _valortiempo)*_escala);
                    Grafico.Children.Insert(0, BarraGrafica(t) );
                    AjustarEscalaGrafico();
                }

                if ((bool) _localSettings.Values["LightMin"])
                {
                    ColorearMinimo();
                }
           
        }

        private Rectangle BarraGrafica(double height)
        {
            var r = new Rectangle
            {
                Tag = _valortiempo,
                Fill = new SolidColorBrush(Colors.Silver),
                Height = height,
                Width = 15,
                Margin = new Thickness(2, 0, 2, 0),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            return r;
        }

        private void CalcularEscalas()
        {
            _valortiempo = _relojPrincipal.ElapsedTicks / 100000;

            if (_valortiempo > _maximo)
                _maximo = _valortiempo;

            if (_valortiempo < _minimo)
                _minimo = _valortiempo;

            if (_maximo == _minimo)
                _escala = 0;
            else
                _escala = TopGraph / (double)(_maximo - _minimo);
        }
        private void ColorearMinimo()
        {            
            foreach (var t in Tiempos.Children.Cast<LapControl>())
            {
                t.Validar(_minimo);
            }
        }

        private void AjustarEscalaGrafico()
        {
            var count = 0;
            foreach (var uiElement in Grafico.Children)
            {
                count++;
                if (count < 26)
                {
                    var r = (Rectangle)uiElement;
                    var t = 1 + TopGraph - ((_maximo - ((long)r.Tag)) * _escala);
                    r.Height = t;
                    
                }
                else
                {
                    break;
                }                
            }
        }

        internal void ReiniciarCronometro()
        {
            _currentLaps = 0;
            TimerTextMain.Text = TimerText.Text = "00:00:00.00";
            _relojVuelta.Reset();
            _relojVuelta.Stop();
            _relojPrincipal.Reset();
            _relojPrincipal.Stop();
            _dt.Stop();

            HMinute.Percentage = HSecond.Percentage = 
            MinuteroSombra.Percentage = HTMinute.Percentage =
            HTSeconds.Percentage = 0.001;            
            Tiempos.Children.Clear();
            Grafico.Children.Clear();
        }
    }

    public static class Conf
    {
        public static bool CortanaInstall = false;
        public static double Div1 = 6.64;
        public static double Div2 = 0.11;
        public static bool Vibrar = false;
        public static bool Sonar = false;
        public static bool Sombra = false;
        public static double ShadowTime = 5;


        public static string TiempoFormato(TimeSpan timerVuelta)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timerVuelta.Hours, timerVuelta.Minutes, timerVuelta.Seconds, timerVuelta.Milliseconds / 10);
        }
    }
}
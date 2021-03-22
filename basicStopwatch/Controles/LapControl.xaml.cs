using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using basicStopWatch.Cronometro;


namespace basicStopWatch.Controles
{
    public sealed partial class LapControl
    {
        public long ValorTiempo;

        private long _ticks;
        public long Ticks
        {
            set
            {
                _ticks = value;                
                Vuelta = Conf.TiempoFormato(new TimeSpan(value));

            }
            get { return _ticks; }
        }
        public LapControl()
        {
            InitializeComponent();
        }

        public int Numero
        {
            set { Num.Text = value.ToString("00"); }
        }

        public string Vuelta
        {
            set { Lap.Text = value; }
        }

        public long Total
        {
            set
            {                
                Main.Text = Conf.TiempoFormato(new TimeSpan(value)); 
            }
        }

        public string Resultado
        {
            get
            {
                return Num.Text + "  " + Lap.Text + "  " + Main.Text;                
            }
        }

        public void Validar(long minimo)
        {
            var str = (ValorTiempo == minimo) ? "PhoneAccentBrush" : "PhoneForegroundBrush";
            Lap.Foreground = (SolidColorBrush) Application.Current.Resources[str];
        }

    }
}
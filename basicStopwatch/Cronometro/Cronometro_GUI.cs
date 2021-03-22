using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


namespace basicStopWatch.Cronometro
{
    public sealed partial class PageCronometro
    {
        private static double SecondColorFactor
        {
            get
            {
                return (((SolidColorBrush) Application.Current.Resources["AppBarBackgroundThemeBrush"]).Color ==
                        Colors.Black)
                    ? .5
                    : 0.5;
            }
        }

        private void ButtonReset_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_relojVuelta.IsRunning)
            {
                AgregarVuelta();
            }
            else
            {
                ReiniciarCronometro();
            }
        }

        private void PausarContinuar(object sender, TappedRoutedEventArgs e)
        {
            if (_relojVuelta.IsRunning)
            {
                Pausar();
            }
            else
            {
                Continuar();
            }
        }

        private void Tiempos_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (Tiempos.Children.Count <= 0) return;
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }        
    }
}
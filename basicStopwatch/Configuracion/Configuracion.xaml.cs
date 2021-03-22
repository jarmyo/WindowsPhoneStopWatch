using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using basicStopWatch.Cronometro;

namespace basicStopWatch.Configuracion
{
    public class DoubleToTextValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string sufijo;
            try
            {
                var context = new ResourceContext();
                context.QualifierValues["Language"] = CultureInfo.CurrentUICulture.Name;
                var resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree(@"Resources");
                sufijo = resourceMap.GetValue(@"ShadowTime", context).ValueAsString;
            }
            catch
            {
                sufijo = "min";
            }
            return value + " " + sufijo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;            
        }
    }


    public sealed partial class Configuracion
    {
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public Configuracion()
        {
            InitializeComponent();
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            var x = (from Control c in Boleanos.Children
                where _localSettings.Values.ContainsKey(c.Name)
                select c).OfType<ToggleSwitch>();

            foreach (var c in x)
            {
                c.IsOn = (bool) _localSettings.Values[c.Name];
            }

            Slider.Value = Convert.ToDouble(_localSettings.Values["Tiempo"]);            
        }

        private void CambiaParametro(object sender, RoutedEventArgs e)
        {
            var c = (ToggleSwitch) sender;
            _localSettings.Values[c.Name] = c.IsOn;

            if (c.Name == "Shadow") Conf.Sombra = (bool)(_localSettings.Values["Shadow"]);
            if (c.Name == "Sound") Conf.Sonar = (bool)(_localSettings.Values["Sound"]);
            if (c.Name == "Vibrate") Conf.Vibrar = (bool)(_localSettings.Values["Vibrate"]);            
        }

        private void ConfirmarCompra(object sender, TappedRoutedEventArgs e)
        {
            Comprar();
        }

        private static async void Comprar()
        {
            try
            {
                MessageDialog messageDialog;

                var context = new ResourceContext();
                context.QualifierValues["Language"] = CultureInfo.CurrentUICulture.Name;
                var resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree(@"Resources");
                var strGraciasPorLaCompra = resourceMap.GetValue(@"PurchaseOK", context).ValueAsString;
                var strNoHuboCompra = resourceMap.GetValue(@"PurchaseNO", context).ValueAsString;

                await CurrentApp.RequestProductPurchaseAsync("Donate", false);

                ProductLicense productLicense;

                if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue("Donate", out productLicense))
                {
                    if (!productLicense.IsActive) return;
                    messageDialog = new MessageDialog(strGraciasPorLaCompra + "!!");
                    await messageDialog.ShowAsync();
                    CurrentApp.ReportProductFulfillment("Donate");
                }
                else
                {
                    messageDialog = new MessageDialog(strNoHuboCompra);
                    await messageDialog.ShowAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void CambiaTiempoSombra(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
            if (_localSettings == null) return;
            if (Slider == null) return;
            Conf.ShadowTime = Slider.Value;
            _localSettings.Values["Tiempo"] = Slider.Value;
            _localSettings.Values["div1"] = Conf.Div1 = 100/(Slider.Value);
            _localSettings.Values["div2"] = Conf.Div2 = Conf.Div1/59;
            }
            catch (Exception e1)
            {
                Debug.WriteLine(e1.Message);
            }
        }
    }
}
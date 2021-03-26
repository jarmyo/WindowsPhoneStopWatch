using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using basicStopWatch.Cronometro;

namespace basicStopWatch
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Conf.CortanaInstall) return;

            if (e.NavigationMode == NavigationMode.New)
            {
                SetCortana();
                Conf.CortanaInstall = true;
            }

        }

        private async void SetCortana()
        {
            await
                      Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(
                          Archivo().Result);
        }


        private async Task<StorageFile> Archivo()
        {
            var dir = new Uri("ms-appx:///Comandos.xml");
            var file = await StorageFile.GetFileFromApplicationUriAsync(dir);
            return file;
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivote.SelectedIndex != 0) return;
            if (Pivote.Items == null) return;
            var pageCronometro = (PageCronometro)((PivotItem)Pivote.Items[0]).Content;
            if (pageCronometro != null)
                pageCronometro.ShowRecordRing();
        }
    }
}

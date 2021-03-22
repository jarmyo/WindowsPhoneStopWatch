using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using basicStopWatch.Cronometro;

namespace basicStopWatch
{
    public sealed partial class App
    {
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private TransitionCollection _transitions;

        public App()
        {
            InitializeComponent();


            SetConfigData();


            Suspending += OnSuspending;
        }

        private void SetSetting(string name, object value)
        {
            if (!_localSettings.Values.ContainsKey(name))
                _localSettings.Values[name] = value;
        }

        private void SetConfigData()
        {

            SetSetting("ResumeCount", false);
            SetSetting("CortanaInstall", false);
            SetSetting("LightMin", true);
            SetSetting("ShowGraph", true);
            SetSetting("Vibrate", true);
            SetSetting("Sound", false);
            SetSetting("Shadow", false);
            SetSetting("RecordRing", false);
            SetSetting("ShadowTime", 5);
            SetSetting("div1", 6.64);
            SetSetting("div2", 0.11);

            Conf.Div1 = Convert.ToDouble(_localSettings.Values["div1"]);
            Conf.Div2 = Convert.ToDouble(_localSettings.Values["div2"]);
            Conf.Sombra = (bool)(_localSettings.Values["Shadow"]);
            Conf.Sonar = (bool)(_localSettings.Values["Sound"]);
            Conf.Vibrar = (bool)(_localSettings.Values["Vibrate"]);
            Conf.CortanaInstall = (bool)(_localSettings.Values["CortanaInstall"]);
        }


        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame { CacheSize = 0 };
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                if (rootFrame.ContentTransitions != null)
                {
                    _transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        _transitions.Add(c);
                    }
                }
                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("No se puede crean Main Page");
                }
            }

            Window.Current.Activate();


        }

        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = (Frame)sender;
            rootFrame.ContentTransitions = _transitions ?? new TransitionCollection { new NavigationThemeTransition() };
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();



            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame { CacheSize = 0 };
                Window.Current.Content = rootFrame;
                rootFrame.Navigate(typeof(MainPage));
            }

            Window.Current.Activate();


            var mp = (MainPage)((Frame)Window.Current.Content).Content;
            if (mp == null || mp.Pivote.Items == null) return;
            var crono = ((PageCronometro)((PivotItem)mp.Pivote.Items[0]).Content);
            if (crono == null) return;

            if (args.Kind != ActivationKind.VoiceCommand) return;

            var commandArgs = args as VoiceCommandActivatedEventArgs;

            if (commandArgs != null)
            {
                var speechRecognitionResult = commandArgs.Result;

                var voiceCommandName = speechRecognitionResult.RulePath[0];

                //var textSpoken = speechRecognitionResult.Text;
                //var navigationTarget = speechRecognitionResult.SemanticInterpretation.Properties["NavigationTarget"][0];

                Comando.Data = voiceCommandName;
            }

            switch (Comando.Data)
            {
                case "NewCount":
                    {
                        crono.ReiniciarCronometro();
                        crono.Continuar();
                        break;
                    }
                case "PauseCount":
                    {
                        crono.Pausar();
                        break;
                    }
                case "ContinueCount":
                    {
                        crono.Continuar();
                        break;
                    }
                case "NewLap":
                    {
                        crono.AgregarVuelta();
                        break;
                    }
            }

        }
    }

    public static class Comando
    {
        public static string Data = ".";
    }

}
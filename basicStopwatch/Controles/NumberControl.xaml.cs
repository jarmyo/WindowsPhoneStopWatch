using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace basicStopWatch.Controles
{
    public sealed partial class NumberControl 
    {
        public NumberControl()
        {
            this.InitializeComponent();
        }

        public int Value { get; set; }

        public string Numero
        {
            set { Number.Text = value; }
            get { return Number.Text; }
        }
    }
}

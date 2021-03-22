#region

using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

#endregion

namespace basicStopWatch.Controles
{
    public partial class CircularProgressBar
    {
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof (double), typeof (CircularProgressBar),
                new PropertyMetadata(65d, OnPercentageChanged));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (int), typeof (CircularProgressBar),
                new PropertyMetadata(5));

        public static readonly DependencyProperty SegmentColorProperty =
            DependencyProperty.Register("SegmentColor", typeof (Brush), typeof (CircularProgressBar),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof (int), typeof (CircularProgressBar),
                new PropertyMetadata(25, OnPropertyChanged));

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof (double), typeof (CircularProgressBar),
                new PropertyMetadata(120d, OnPropertyChanged));

        public CircularProgressBar()
        {
            InitializeComponent();
            Angle = (Percentage*360)/100;
            RenderArc();
        }

        public int Radius
        {
            get { return (int) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public Brush SegmentColor
        {
            get { return (Brush) GetValue(SegmentColorProperty); }
            set { SetValue(SegmentColorProperty, value); }
        }

        public int StrokeThickness
        {
            get { return (int) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public double Percentage
        {
            get { return (double) GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        private static void OnPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var circle = sender as CircularProgressBar;
            if (circle != null) circle.Angle = (circle.Percentage*360)/100;
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var circle = sender as CircularProgressBar;
            if (circle != null) circle.RenderArc();
        }

        public void RenderArc()
        {
            var startPoint = new Point(Radius, 0);
            var endPoint = ComputeCartesianCoordinate(Angle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            PathRoot.Width = Radius*2 + StrokeThickness;
            PathRoot.Height = Radius*2 + StrokeThickness;
            PathRoot.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

            var largeArc = Angle > 180.0;

            var outerArcSize = new Size(Radius, Radius);

            PathFigure.StartPoint = startPoint;

            if (Math.Abs(startPoint.X - Math.Round(endPoint.X)) < 0 && Math.Abs(startPoint.Y - Math.Round(endPoint.Y)) < 0)
                endPoint.X -= 0.01;

            ArcSegment.Point = endPoint;
            ArcSegment.Size = outerArcSize;
            ArcSegment.IsLargeArc = largeArc;
        }

        private static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            var angleRad = (Math.PI/180.0)*(angle - 90);

            var x = radius*Math.Cos(angleRad);
            var y = radius*Math.Sin(angleRad);

            return new Point(x, y);
        }
    }
}
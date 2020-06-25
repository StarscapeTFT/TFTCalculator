using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace TFTCalculator
{
    public class FancyConcaveBorder : Decorator
    {
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Brush OuterBorderBrush
        {
            get { return (Brush)GetValue(OuterBorderBrushProperty); }
            set { SetValue(OuterBorderBrushProperty, value); }
        }

        public Brush InnerBorderBrush
        {
            get { return (Brush)GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        public double OuterBorderThickness
        {
            get { return (double)GetValue(OuterBorderThicknessProperty); }
            set { SetValue(OuterBorderThicknessProperty, value); }
        }

        public double InnerBorderThickness
        {
            get { return (double)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }


        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(FancyConcaveBorder), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty OuterBorderBrushProperty = DependencyProperty.Register("OuterBorderBrush", typeof(Brush), typeof(FancyConcaveBorder), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty InnerBorderBrushProperty = DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(FancyConcaveBorder), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty OuterBorderThicknessProperty = DependencyProperty.Register("OuterBorderThickness", typeof(double), typeof(FancyConcaveBorder), new UIPropertyMetadata(2.0));
        public static readonly DependencyProperty InnerBorderThicknessProperty = DependencyProperty.Register("InnerBorderThickness", typeof(double), typeof(FancyConcaveBorder), new UIPropertyMetadata(1.0));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(FancyConcaveBorder), new UIPropertyMetadata(1.0));

        protected override void OnRender(DrawingContext dc)
        {
            Pen outerPen = new Pen(OuterBorderBrush, OuterBorderThickness);

            double w = RenderSize.Width;
            double h = RenderSize.Height;

            // OUTER BORDER
            PathFigure p = new PathFigure(new Point(CornerRadius, 0), new PathSegment[] { }, true);

            p.Segments.Add(new LineSegment(new Point(w - CornerRadius, 0), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(w, CornerRadius), new Size(CornerRadius, CornerRadius), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(w, h - CornerRadius), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(w - CornerRadius, h), new Size(CornerRadius, CornerRadius), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(CornerRadius, h), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(0, h - CornerRadius), new Size(CornerRadius, CornerRadius), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(0, CornerRadius), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(CornerRadius, 0), new Size(CornerRadius, CornerRadius), 0, false, SweepDirection.Counterclockwise, true));

            Geometry g = new PathGeometry(new PathFigure[] { p });

            // Help round the drawing to the nearest pixel so it doesn't look blurry.
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(0 - OuterBorderThickness / 2);
            guidelines.GuidelinesX.Add(0 + OuterBorderThickness / 2);
            guidelines.GuidelinesX.Add(w - OuterBorderThickness / 2);
            guidelines.GuidelinesX.Add(w + OuterBorderThickness / 2);
            guidelines.GuidelinesY.Add(0 - OuterBorderThickness / 2);
            guidelines.GuidelinesY.Add(0 + OuterBorderThickness / 2);
            guidelines.GuidelinesY.Add(h - OuterBorderThickness / 2);
            guidelines.GuidelinesY.Add(h + OuterBorderThickness / 2);
            dc.PushGuidelineSet(guidelines);
            dc.DrawGeometry(Background, outerPen, g);
            dc.Pop();

            // INNER BORDER
            Pen innerPen = new Pen(InnerBorderBrush, InnerBorderThickness);
            double o = OuterBorderThickness + InnerBorderThickness; // o = offset
            p = new PathFigure(new Point(CornerRadius + o, o), new PathSegment[] { }, true);

            p.Segments.Add(new LineSegment(new Point(w - CornerRadius - o, o), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(w - o, CornerRadius + o), new Size(CornerRadius + o, CornerRadius + o), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(w - o, h - CornerRadius - o), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(w - CornerRadius - o, h - o), new Size(CornerRadius + o, CornerRadius + o), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(CornerRadius + o, h - o), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(o, h - CornerRadius - o), new Size(CornerRadius + o, CornerRadius + o), 0, false, SweepDirection.Counterclockwise, true));
            p.Segments.Add(new LineSegment(new Point(o, CornerRadius + o), true));
            if (CornerRadius > 0) p.Segments.Add(new ArcSegment(new Point(CornerRadius + o, o), new Size(CornerRadius + o, CornerRadius + o), 0, false, SweepDirection.Counterclockwise, true));
            g = new PathGeometry(new PathFigure[] { p });

            guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(o - InnerBorderThickness / 2);
            guidelines.GuidelinesX.Add(o + InnerBorderThickness / 2);
            guidelines.GuidelinesX.Add(w - o - InnerBorderThickness / 2);
            guidelines.GuidelinesX.Add(w - o + InnerBorderThickness / 2);
            guidelines.GuidelinesY.Add(o - InnerBorderThickness / 2);
            guidelines.GuidelinesY.Add(o + InnerBorderThickness / 2);
            guidelines.GuidelinesY.Add(h - o - InnerBorderThickness / 2);
            guidelines.GuidelinesY.Add(h - o + InnerBorderThickness / 2);
            dc.PushGuidelineSet(guidelines);
            dc.DrawGeometry(Background, innerPen, g);
            dc.Pop();
        }
    }
}

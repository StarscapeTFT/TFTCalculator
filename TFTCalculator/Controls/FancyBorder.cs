using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace TFTCalculator
{
    public class FancyBorder : Border
    {
        protected override void OnRender(DrawingContext dc)
        {
            Pen leftPen = new Pen(BorderBrush, BorderThickness.Left);
            Pen topPen = new Pen(BorderBrush, BorderThickness.Top);
            Pen rightPen = new Pen(BorderBrush, BorderThickness.Right);
            Pen bottomPen = new Pen(BorderBrush, BorderThickness.Bottom);

            double w = RenderSize.Width;
            double h = RenderSize.Height;

            // Top and bottom frills
            double frillStart = 8;
            double frillStop = w - 8;

            if (frillStop - frillStart - 8 > 0)
            {
                // Top frill
                PathFigure p1 = new PathFigure(new Point(frillStart, 0), new PathSegment[] { }, false);

                p1.Segments.Add(new LineSegment(new Point(frillStart + 4, -4), true));
                p1.Segments.Add(new LineSegment(new Point(frillStop - 4, -4), true));
                p1.Segments.Add(new LineSegment(new Point(frillStop, 0), true));

                GuidelineSet frillGuidelines = new GuidelineSet();
                frillGuidelines.GuidelinesY.Add(-4 - BorderThickness.Top / 2);
                frillGuidelines.GuidelinesY.Add(-4 + BorderThickness.Top / 2);

                PathFigure p2 = new PathFigure(new Point(frillStart, h), new PathSegment[] { }, false);

                p2.Segments.Add(new LineSegment(new Point(frillStart + 4, h + 4), true));
                p2.Segments.Add(new LineSegment(new Point(frillStop - 4, h + 4), true));
                p2.Segments.Add(new LineSegment(new Point(frillStop, h), true));

                frillGuidelines.GuidelinesY.Add(h + 4 - BorderThickness.Top / 2);
                frillGuidelines.GuidelinesY.Add(h + 4 + BorderThickness.Top / 2);

                PathGeometry g = new PathGeometry(new PathFigure[] { p1, p2 });
                dc.PushGuidelineSet(frillGuidelines);
                dc.DrawGeometry(Background, topPen, g);
                dc.Pop();
            }

            // Main border square
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(0 - BorderThickness.Left / 2);
            guidelines.GuidelinesX.Add(0 + BorderThickness.Left / 2);
            guidelines.GuidelinesX.Add(w - BorderThickness.Right / 2);
            guidelines.GuidelinesX.Add(w + BorderThickness.Right / 2);
            guidelines.GuidelinesY.Add(0 - BorderThickness.Top / 2);
            guidelines.GuidelinesY.Add(0 + BorderThickness.Top / 2);
            guidelines.GuidelinesY.Add(h - BorderThickness.Bottom / 2);
            guidelines.GuidelinesY.Add(h + BorderThickness.Bottom / 2);
            dc.PushGuidelineSet(guidelines);
            dc.DrawRectangle(Background, topPen, new Rect(0, 0, w, h));
            dc.Pop();
        }
    }
}

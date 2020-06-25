using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TFTCalculator
{
    public class PressableTabItem : TabItem
    {
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }

        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(PressableTabItem), new PropertyMetadata(false));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsPressed = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsPressed = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsPressed = false;
            IsSelected = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            double mouseX = e.GetPosition(this).X;

            double halfWidth = ActualWidth / 2;

            if (GetTemplateChild("PART_Glow") is FrameworkElement glow
                && GetTemplateChild("PART_GlowBorder") is FrameworkElement glowBorder)
            {
                double midX = (mouseX - halfWidth) / (ActualWidth);
                double y = Math.Abs(glow.ActualHeight * Math.Sin(midX));

                double xOffset = (mouseX - halfWidth) * Math.Cos(midX);
                double x = halfWidth + xOffset;

                glow.RenderTransform = new TranslateTransform(x, y);
                glowBorder.RenderTransform = new TranslateTransform(x, 0);
                glowBorder.Opacity = 1 - Math.Abs(1.5 * Math.Sin(midX));
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TFTCalculator
{

    [TemplatePart(Name = "Part_TextBox", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "Part_UpButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "Part_DownButton", Type = typeof(RepeatButton))]
    public class IntegerUpDown : Control
    {
        static IntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(typeof(IntegerUpDown)));
            MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(0));
            MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(int.MaxValue));
            StepProperty = DependencyProperty.Register("StepValue", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(1));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IntegerUpDown), new UIPropertyMetadata("0"));
        }

        private TextBox textBoxElement;
        private RepeatButton upButtonElement;
        private RepeatButton downButtonElement;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty MinimumProperty;
        public static readonly DependencyProperty MaximumProperty;
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty StepProperty;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int StepValue
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (upButtonElement != null)
                upButtonElement.Click -= UpButtonElement_Click;

            if (downButtonElement != null)
                downButtonElement.Click -= DownButtonElement_Click;

            if (textBoxElement != null)
                textBoxElement.TextChanged -= TextBoxElement_TextChanged;

            base.OnApplyTemplate();
            textBoxElement = Template.FindName("Part_TextBox", this) as TextBox;
            upButtonElement = Template.FindName("Part_UpButton", this) as RepeatButton;
            downButtonElement = Template.FindName("Part_DownButton", this) as RepeatButton;

            textBoxElement.Text = Text;

            if (upButtonElement != null)
                upButtonElement.Click += UpButtonElement_Click;

            if (downButtonElement != null)
                downButtonElement.Click += DownButtonElement_Click;

            if (textBoxElement != null)
                textBoxElement.TextChanged += TextBoxElement_TextChanged;
        }

        private void TextBoxElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = textBoxElement.Text.ToString();
        }

        private void UpButtonElement_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Text, out int Value))
            {
                if (Value < Maximum - StepValue)
                {
                    if (Value + StepValue < Minimum)
                    {
                        Value = Minimum;
                    }
                    else
                    {
                        Value += StepValue;
                    }
                }
                else
                {
                    Value = Maximum;
                }

                textBoxElement.Text = Value.ToString();
            }
        }

        private void DownButtonElement_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Text, out int Value))
            {
                if (Value > Minimum + StepValue)
                {
                    if (Value - StepValue > Maximum)
                    {
                        Value = Maximum;
                    }
                    else
                    {
                        Value -= StepValue;
                    }
                }
                else
                {
                    Value = Minimum;
                }

                textBoxElement.Text = Value.ToString();
            }
        }
    }
}

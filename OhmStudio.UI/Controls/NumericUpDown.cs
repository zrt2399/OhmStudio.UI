using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Controls
{
    public class NumericUpDown : Control
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
        {
            GotFocus += NumericUpDown_GotFocus;
            PreviewKeyDown += NumericUpDown_KeyDown;
            PreviewMouseWheel += NumericUpDown_PreviewMouseWheel;
            IncreaseCommand = new RelayCommand(Increase);
            ReduceCommand = new RelayCommand(Reduce);
        }

        TextBox PART_TextBox;

        internal static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register(nameof(ValueText), typeof(string), typeof(NumericUpDown), new PropertyMetadata("0"));

        internal static readonly DependencyProperty IsIncreaseEnabledProperty =
            DependencyProperty.Register(nameof(IsIncreaseEnabled), typeof(bool), typeof(NumericUpDown));

        internal static readonly DependencyProperty IsReduceEnabledProperty =
            DependencyProperty.Register(nameof(IsReduceEnabled), typeof(bool), typeof(NumericUpDown));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumericUpDown), new PropertyMetadata(1d));

        public static readonly DependencyProperty IsMouseWheelProperty =
            DependencyProperty.Register(nameof(IsMouseWheel), typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true, (sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown && e.NewValue is bool newValue)
                {
                    numericUpDown.PreviewMouseWheel -= NumericUpDown_PreviewMouseWheel;
                    if (newValue)
                    {
                        numericUpDown.PreviewMouseWheel += NumericUpDown_PreviewMouseWheel;
                    }
                }
            }));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(NumericUpDown));

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(NumericUpDown), new PropertyMetadata("{0}", (sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown)
                {
                    numericUpDown.UpdateValueText(numericUpDown.Value);
                }
            }));

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(nameof(ValueFormat), typeof(string), typeof(NumericUpDown), new PropertyMetadata((sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown)
                {
                    numericUpDown.UpdateValueText(numericUpDown.Value);
                }
            }));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MaxValue, (sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown && e.NewValue is double newValue)
                {
                    if (numericUpDown.Value > newValue)
                    {
                        numericUpDown.Value = newValue;
                    }
                    CoerceRange(numericUpDown);
                    numericUpDown.IsIncreaseEnabled = numericUpDown.Value < newValue;
                }
            }));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MinValue, (sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown && e.NewValue is double newValue)
                {
                    if (numericUpDown.Value < newValue)
                    {
                        numericUpDown.Value = newValue;
                    }
                    CoerceRange(numericUpDown);
                    numericUpDown.IsReduceEnabled = numericUpDown.Value > newValue;
                }
            }));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is NumericUpDown numericUpDown && e.NewValue is double newValue)
                {
                    numericUpDown.UpdateValueText(newValue);
                }
            }));

        internal string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }

        internal bool IsIncreaseEnabled
        {
            get => (bool)GetValue(IsIncreaseEnabledProperty);
            set => SetValue(IsIncreaseEnabledProperty, value);
        }

        internal bool IsReduceEnabled
        {
            get => (bool)GetValue(IsReduceEnabledProperty);
            set => SetValue(IsReduceEnabledProperty, value);
        }

        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public bool IsMouseWheel
        {
            get => (bool)GetValue(IsMouseWheelProperty);
            set => SetValue(IsMouseWheelProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        public string ValueFormat
        {
            get => (string)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ICommand IncreaseCommand { get; }

        public ICommand ReduceCommand { get; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PART_TextBox != null)
            {
                PART_TextBox.LostFocus -= PART_TextBox_LostFocus;
            }
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_TextBox.LostFocus += PART_TextBox_LostFocus;
        }

        public void Increase()
        {
            var temp = (decimal)Value;
            temp += (decimal)Increment;
            Value = (double)temp;
        }

        public void Reduce()
        {
            var temp = (decimal)Value;
            temp -= (decimal)Increment;
            Value = (double)temp;
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (double.TryParse(textBox.Text.Trim(), out var result))
            {
                if (Value == result)
                {
                    UpdateValueText(result);
                }
                Value = result;
            }
            else
            {
                //ValueText = string.Format(FormatString, Value.ToString(Format));
                UpdateValueText(Value);
            }
        }

        private static void CoerceRange(NumericUpDown numericUpDown)
        {
            if (numericUpDown.Minimum > numericUpDown.Maximum)
            {
                numericUpDown.Maximum = numericUpDown.Minimum;
            }
        }

        private void UpdateValueText(double newValue)
        {
            if (Value < Minimum)
            {
                Value = Minimum;
            }
            else if (Value > Maximum)
            {
                Value = Maximum;
            }
            else
            {
                var integer = (long)newValue;
                ValueText = string.Format(StringFormat, integer == newValue ? integer.ToString(ValueFormat) : newValue.ToString(ValueFormat));

                IsIncreaseEnabled = newValue < Maximum;
                IsReduceEnabled = newValue > Minimum;
            }
        }

        private void NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                Increase();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                Reduce();
                e.Handled = true;
            }
        }

        private static void NumericUpDown_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var numericUpDown = (NumericUpDown)sender;
            if (e.Delta > 0)
            {
                numericUpDown.Increase();
            }
            else
            {
                numericUpDown.Reduce();
            }
        }

        private void NumericUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    PART_TextBox.Focus();
                    PART_TextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }
    }
}
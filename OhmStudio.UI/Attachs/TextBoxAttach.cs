using System.Windows;
using System.Windows.Controls;
using OhmStudio.UI.PublicMethods;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.Attachs
{
    public class TextBoxAttach
    {
        public const string PlaceHolder = null;
        public const double PlaceHolderOpacity = 0.6;
        public const bool PlaceHolderIsHitTestVisible = false;

        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.RegisterAttached("Title", typeof(object), typeof(TextBoxAttach));

        public static object GetTitle(DependencyObject target)
        {
            return target.GetValue(TitleProperty);
        }

        public static void SetTitle(DependencyObject target, object value)
        {
            target.SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleIsHitTestVisibleProperty =
            DependencyProperty.RegisterAttached("TitleIsHitTestVisible", typeof(bool), typeof(TextBoxAttach));

        public static bool GetTitleIsHitTestVisible(DependencyObject target)
        {
            return (bool)target.GetValue(TitleIsHitTestVisibleProperty);
        }

        public static void SetTitleIsHitTestVisible(DependencyObject target, bool value)
        {
            target.SetValue(TitleIsHitTestVisibleProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(object), typeof(TextBoxAttach), new PropertyMetadata(null, (sender, e) =>
            {
                var newValue = e.NewValue;
                if (sender.IsTextBoxAttachObject() && CheckIsEmpty(newValue))
                {
                    SetPlaceHolderVisibility(sender, Visibility.Collapsed);
                }
                if (sender is ComboBox comboBox)
                {
                    comboBox.Loaded += ComboBox_Loaded;
                }
                else if (sender is TextBox textBox)
                {
                    textBox.TextChanged -= TextBox_TextChanged;
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(textBox, textBox.Text);
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                }
                else if (sender is PasswordBox passwordBox)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(passwordBox, passwordBox.Password);
                        passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    }
                }
                else if (sender is PasswordBoxControl passwordBoxControl)
                {
                    passwordBoxControl.txtPassword.PasswordChanged -= PasswordBoxControl_PasswordChanged;
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(passwordBoxControl, passwordBoxControl.txtPassword.Password);
                        passwordBoxControl.txtPassword.PasswordChanged += PasswordBoxControl_PasswordChanged;
                    }
                }
                else if (sender is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.textBoxDateTime.TextChanged -= DateTimePicker_TextChanged;
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(dateTimePicker, dateTimePicker.textBoxDateTime.Text);
                        dateTimePicker.textBoxDateTime.TextChanged += DateTimePicker_TextChanged;
                    }
                }
            }));

        private static bool CheckIsEmpty(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            else if (obj is string value)
            {
                return string.IsNullOrWhiteSpace(value);
            }
            return false;
        }

        private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Loaded -= ComboBox_Loaded;
            comboBox.Dispatcher.InvokeAsync(() =>
            {
                var textBox = comboBox.Template?.FindName("PART_EditableTextBox", comboBox) as TextBox;
                if (textBox != null)
                {
                    textBox.TextChanged -= ComboBoxTextBox_TextChanged;
                    comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                    if (!CheckIsEmpty(GetPlaceHolder(comboBox)))
                    {
                        UpdateHolderVisibility(comboBox, comboBox.IsEditable ? textBox.Text : comboBox.SelectionBoxItem?.ToString());
                        textBox.TextChanged += ComboBoxTextBox_TextChanged;
                        comboBox.SelectionChanged += ComboBox_SelectionChanged;
                    }
                }
            });
        }

        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (!comboBox.IsEditable)
            {
                UpdateHolderVisibility(comboBox, comboBox.SelectedItem?.ToString());
            }
        }

        private static void ComboBoxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            ComboBox comboBox = textBox.FindParentObject<ComboBox>();
            if (comboBox.IsEditable)
            {
                UpdateHolderVisibility(textBox.FindParentObject<ComboBox>(), textBox.Text);
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            UpdateHolderVisibility(passwordBox, passwordBox.Password);
        }

        private static void PasswordBoxControl_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            UpdateHolderVisibility(passwordBox.FindParentFrameworkElement<PasswordBoxControl>(), passwordBox.Password);
        }

        private static void DateTimePicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateHolderVisibility(textBox.FindParentFrameworkElement<DateTimePicker>(), textBox.Text);
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateHolderVisibility(textBox, textBox.Text);
        }

        static void UpdateHolderVisibility(DependencyObject target, string value)
        {
            if (target != null)
            {
                SetPlaceHolderVisibility(target, string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public static object GetPlaceHolder(DependencyObject target)
        {
            return target.GetValue(PlaceHolderProperty);
        }

        public static void SetPlaceHolder(DependencyObject target, object value)
        {
            target.SetValue(PlaceHolderProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderIsHitTestVisibleProperty =
           DependencyProperty.RegisterAttached("PlaceHolderIsHitTestVisible", typeof(bool), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolderIsHitTestVisible));

        public static bool GetPlaceHolderIsHitTestVisible(DependencyObject target)
        {
            return (bool)target.GetValue(PlaceHolderIsHitTestVisibleProperty);
        }

        public static void SetPlaceHolderIsHitTestVisible(DependencyObject target, bool value)
        {
            target.SetValue(PlaceHolderIsHitTestVisibleProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderOpacityProperty =
            DependencyProperty.RegisterAttached("PlaceHolderOpacity", typeof(double), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolderOpacity));
        public static double GetPlaceHolderOpacity(DependencyObject target)
        {
            return (double)target.GetValue(PlaceHolderOpacityProperty);
        }

        public static void SetPlaceHolderOpacity(DependencyObject target, double value)
        {
            target.SetValue(PlaceHolderOpacityProperty, value);
        }

        internal static readonly DependencyProperty PlaceHolderVisibilityProperty =
            DependencyProperty.RegisterAttached("PlaceHolderVisibility", typeof(Visibility), typeof(TextBoxAttach), new PropertyMetadata(Visibility.Collapsed));
        internal static Visibility GetPlaceHolderVisibility(DependencyObject target)
        {
            return (Visibility)target.GetValue(PlaceHolderVisibilityProperty);
        }

        internal static void SetPlaceHolderVisibility(DependencyObject target, Visibility value)
        {
            target.SetValue(PlaceHolderVisibilityProperty, value);
        }
    }
}
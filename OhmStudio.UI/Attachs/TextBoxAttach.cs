using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OhmStudio.UI.PublicMethod;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.Attachs
{
    public class TextBoxAttach
    {
        public const string PlaceHolder = "";
        public static readonly Brush PlaceHolderForeground = "#FF999999".ToSolidColorBrush();
        public const double PlaceHolderOpacity = 1d;
        public static readonly Thickness PlaceHolderMargin = new Thickness(2, 0, 2, 0);
        public const Visibility PlaceHolderVisibility = Visibility.Collapsed;

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolder, (sender, e) =>
            {
                if (e.NewValue is string value)
                {
                    if (sender is TextBox textBox)
                    {
                        textBox.TextChanged -= TextBox_TextChanged;
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            SetPlaceHolderVisibility(textBox, Visibility.Collapsed);
                        }
                        else
                        {
                            UpdateHolderVisibility(textBox, textBox.Text);
                            textBox.TextChanged += TextBox_TextChanged;
                        }
                    }
                    else if (sender is PasswordBoxControl passwordBoxControl)
                    {
                        passwordBoxControl.txtPassword.PasswordChanged -= TxtPassword_PasswordChanged;
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            SetPlaceHolderVisibility(passwordBoxControl, Visibility.Collapsed);
                        }
                        else
                        {
                            UpdateHolderVisibility(passwordBoxControl, passwordBoxControl.Password);
                            passwordBoxControl.txtPassword.PasswordChanged += TxtPassword_PasswordChanged;
                        }
                    }
                    else if (sender is DateTimePicker dateTimePicker)
                    {
                        dateTimePicker.textBoxDateTime.TextChanged -= DateTimePicker_TextChanged;
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            SetPlaceHolderVisibility(dateTimePicker, Visibility.Collapsed);
                        }
                        else
                        {
                            UpdateHolderVisibility(dateTimePicker, dateTimePicker.textBoxDateTime.Text);
                            dateTimePicker.textBoxDateTime.TextChanged += DateTimePicker_TextChanged;
                        }
                    }
                }
            }));

        private static void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            UpdateHolderVisibility(passwordBox.FindParentObject<PasswordBoxControl>(), passwordBox.Password);
        }

        private static void DateTimePicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateHolderVisibility(textBox.FindParentObject<DateTimePicker>(), textBox.Text);
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
            //var SSS = GetPlaceHolderVisibility(target); 
        }

        public static string GetPlaceHolder(DependencyObject target)
        {
            return (string)target.GetValue(PlaceHolderProperty);
        }

        public static void SetPlaceHolder(DependencyObject target, string value)
        {
            target.SetValue(PlaceHolderProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderForegroundProperty =
            DependencyProperty.RegisterAttached("PlaceHolderForeground", typeof(Brush), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolderForeground));
        public static Brush GetPlaceHolderForeground(DependencyObject target)
        {
            return (Brush)target.GetValue(PlaceHolderForegroundProperty);
        }

        public static void SetPlaceHolderForeground(DependencyObject target, Brush value)
        {
            target.SetValue(PlaceHolderForegroundProperty, value);
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

        public static readonly DependencyProperty PlaceHolderMarginProperty =
            DependencyProperty.RegisterAttached("PlaceHolderMargin", typeof(Thickness), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolderMargin));
        public static Thickness GetPlaceHolderMargin(DependencyObject target)
        {
            return (Thickness)target.GetValue(PlaceHolderMarginProperty);
        }

        public static void SetPlaceHolderMargin(DependencyObject target, Thickness value)
        {
            target.SetValue(PlaceHolderMarginProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderVisibilityProperty =
            DependencyProperty.RegisterAttached("PlaceHolderVisibility", typeof(Visibility), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolderVisibility));
        public static Visibility GetPlaceHolderVisibility(DependencyObject target)
        {
            return (Visibility)target.GetValue(PlaceHolderVisibilityProperty);
        }

        public static void SetPlaceHolderVisibility(DependencyObject target, Visibility value)
        {
            target.SetValue(PlaceHolderVisibilityProperty, value);
        }
    }
}
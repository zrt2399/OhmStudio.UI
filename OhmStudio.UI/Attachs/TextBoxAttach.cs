using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OhmStudio.UI.PublicMethod;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.Attachs
{
    public class TextBoxAttachBase
    {
        //public const string PlaceHolder = "";
        public static readonly Brush Foreground = "#FF999999".ToSolidColorBrush();
        public const double Opacity = 1d;
        public static readonly Thickness Margin = new Thickness(2, 0, 2, 0);
        //public const Visibility Visibility1 = Visibility.Collapsed;

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(TextBoxAttachBase), new PropertyMetadata(Foreground));
        public static Brush GetForeground(DependencyObject target)
        {
            return (Brush)target.GetValue(ForegroundProperty);
        }

        public static void SetForeground(DependencyObject target, Brush value)
        {
            target.SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.RegisterAttached("Opacity", typeof(double), typeof(TextBoxAttachBase), new PropertyMetadata(Opacity));
        public static double GetOpacity(DependencyObject target)
        {
            return (double)target.GetValue(OpacityProperty);
        }

        public static void SetOpacity(DependencyObject target, double value)
        {
            target.SetValue(OpacityProperty, value);
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(TextBoxAttachBase), new PropertyMetadata(Margin));
        public static Thickness GetMargin(DependencyObject target)
        {
            return (Thickness)target.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject target, Thickness value)
        {
            target.SetValue(MarginProperty, value);
        }

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(Visibility), typeof(TextBoxAttachBase), new PropertyMetadata(Visibility.Collapsed));
        public static Visibility GetVisibility(DependencyObject target)
        {
            return (Visibility)target.GetValue(VisibilityProperty);
        }

        public static void SetVisibility(DependencyObject target, Visibility value)
        {
            target.SetValue(VisibilityProperty, value);
        }
    }

    public class TextBoxPlaceholderAttach : TextBoxAttachBase
    {
        public const string PlaceHolder = "";

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(TextBoxPlaceholderAttach), new PropertyMetadata(PlaceHolder, (sender, e) =>
            {
                if (e.NewValue is string newValue)
                {
                    if (sender.IsTextBoxAttachObject() && string.IsNullOrWhiteSpace(newValue))
                    {
                        SetVisibility(sender, Visibility.Collapsed);
                    }
                    if (sender is TextBox textBox)
                    {
                        textBox.TextChanged -= TextBox_TextChanged;
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            UpdateHolderVisibility(textBox, textBox.Text);
                            textBox.TextChanged += TextBox_TextChanged;
                        }
                    }
                    else if (sender is PasswordBox passwordBox)
                    {
                        passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            UpdateHolderVisibility(passwordBox, passwordBox.Password);
                            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                        }
                    }
                    else if (sender is PasswordBoxControl passwordBoxControl)
                    {
                        passwordBoxControl.txtPassword.PasswordChanged -= PasswordBoxControl_PasswordChanged;
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            UpdateHolderVisibility(passwordBoxControl, passwordBoxControl.Password);
                            passwordBoxControl.txtPassword.PasswordChanged += PasswordBoxControl_PasswordChanged;
                        }
                    }
                    else if (sender is DateTimePicker dateTimePicker)
                    {
                        dateTimePicker.textBoxDateTime.TextChanged -= DateTimePicker_TextChanged;
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            UpdateHolderVisibility(dateTimePicker, dateTimePicker.textBoxDateTime.Text);
                            dateTimePicker.textBoxDateTime.TextChanged += DateTimePicker_TextChanged;
                        }
                    }
                }
            }));

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            UpdateHolderVisibility(passwordBox, passwordBox.Password);
        }

        private static void PasswordBoxControl_PasswordChanged(object sender, RoutedEventArgs e)
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
                SetVisibility(target, string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed);
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
    }

    public class TextBoxTitleAttach : TextBoxAttachBase
    {
        public const string Title = "";

        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.RegisterAttached("Title", typeof(string), typeof(TextBoxTitleAttach), new PropertyMetadata(Title, (sender, e) =>
           {
               if (e.NewValue is string newValue && sender.IsTextBoxAttachObject())
               {
                   if (string.IsNullOrEmpty(newValue))
                   {
                       SetVisibility(sender, Visibility.Collapsed);
                   }
                   else
                   {
                       SetVisibility(sender, Visibility.Visible);
                   }
               }
           }));

        public static string GetTitle(DependencyObject target)
        {
            return (string)target.GetValue(TitleProperty);
        }

        public static void SetTitle(DependencyObject target, string value)
        {
            target.SetValue(TitleProperty, value);
        }
    }
}
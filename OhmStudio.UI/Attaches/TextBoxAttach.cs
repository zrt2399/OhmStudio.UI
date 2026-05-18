using System.Windows;
using System.Windows.Controls;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Attaches
{
    public class TextBoxAttach
    {
        public const string PlaceHolder = "";
        public const double PlaceHolderOpacity = 0.6;

        public static readonly DependencyProperty HeaderPlacementProperty =
            DependencyProperty.RegisterAttached("HeaderPlacement", typeof(Dock), typeof(TextBoxAttach));

        public static Dock GetHeaderPlacement(DependencyObject target)
        {
            return (Dock)target.GetValue(HeaderPlacementProperty);
        }

        public static void SetHeaderPlacement(DependencyObject target, Dock value)
        {
            target.SetValue(HeaderPlacementProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("Header", typeof(object), typeof(TextBoxAttach));

        public static object GetHeader(DependencyObject target)
        {
            return target.GetValue(HeaderProperty);
        }

        public static void SetHeader(DependencyObject target, object value)
        {
            target.SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderTextProperty =
            DependencyProperty.RegisterAttached("PlaceHolderText", typeof(string), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolder, (sender, e) =>
            {
                var newValue = e.NewValue as string;
                if (sender.IsPlaceHolderObject() && string.IsNullOrEmpty(newValue))
                {
                    SetPlaceHolderVisibility(sender, Visibility.Collapsed);
                }
                var type = sender.GetType();
                if (type == typeof(ComboBox) && sender is ComboBox comboBox)
                {
                    if (comboBox.IsLoaded)
                    {
                        InvokePlaceHolderChanged(comboBox);
                    }
                    else
                    {
                        comboBox.Loaded += ComboBox_Loaded;
                    }
                }
                else if (type == typeof(TextBox) && sender is TextBox textBox)
                {
                    textBox.TextChanged -= TextBox_TextChanged;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        UpdateHolderVisibility(textBox, textBox.Text);
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                }
                else if (type == typeof(PasswordBox) && sender is PasswordBox passwordBox)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        UpdateHolderVisibility(passwordBox, passwordBox.Password);
                        passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    }
                }
                else if (sender is ITextChanged textChanged)
                {
                    textChanged.TextChanged -= ITextChanged;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        UpdateHolderVisibility(sender, textChanged.Text);
                        textChanged.TextChanged += ITextChanged;
                    }
                }
            }));

        private static void ITextChanged(object sender, TextChangedEventArgs e)
        {
            var dependencyObject = sender as DependencyObject;
            var textBox = e.Source as TextBox;
            UpdateHolderVisibility(dependencyObject, textBox?.Text);
        }

        private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Loaded -= ComboBox_Loaded;
                InvokePlaceHolderChanged(comboBox);
            }
        }

        private static void InvokePlaceHolderChanged(ComboBox comboBox)
        {
            if (comboBox.Template?.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
            {
                textBox.TextChanged -= ComboBoxTextBox_TextChanged;
                comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                if (!string.IsNullOrEmpty(GetPlaceHolderText(comboBox)))
                {
                    UpdateHolderVisibility(comboBox, comboBox.IsEditable ? textBox.Text : comboBox.SelectedItem?.ToString());
                    textBox.TextChanged += ComboBoxTextBox_TextChanged;
                    comboBox.SelectionChanged += ComboBox_SelectionChanged;
                }
            }
            else
            {
                comboBox.Loaded += ComboBox_Loaded;
            }
        }

        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox { IsEditable: false } comboBox)
            {
                UpdateHolderVisibility(comboBox, comboBox.SelectedItem?.ToString());
            }
        }

        private static void ComboBoxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox { TemplatedParent: ComboBox { IsEditable: true } comboBox } textBox)
            {
                UpdateHolderVisibility(comboBox, textBox.Text);
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                UpdateHolderVisibility(textBox, textBox.Text);
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                UpdateHolderVisibility(passwordBox, passwordBox.Password);
            }
        }

        private static void UpdateHolderVisibility(DependencyObject target, string value)
        {
            if (target != null)
            {
                SetPlaceHolderVisibility(target, string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public static string GetPlaceHolderText(DependencyObject target)
        {
            return (string)target.GetValue(PlaceHolderTextProperty);
        }

        public static void SetPlaceHolderText(DependencyObject target, string value)
        {
            target.SetValue(PlaceHolderTextProperty, value);
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

        public static readonly DependencyProperty ShowValidationErrorInfoProperty =
            DependencyProperty.RegisterAttached("ShowValidationErrorInfo", typeof(bool), typeof(TextBoxAttach), new PropertyMetadata(true));
        public static bool GetShowValidationErrorInfo(DependencyObject target)
        {
            return (bool)target.GetValue(ShowValidationErrorInfoProperty);
        }

        public static void SetShowValidationErrorInfo(DependencyObject target, bool value)
        {
            target.SetValue(ShowValidationErrorInfoProperty, value);
        }

        /// <summary>
        /// SuffixText dependency property.
        /// </summary>
        public static readonly DependencyProperty SuffixTextProperty =
            DependencyProperty.RegisterAttached("SuffixText", typeof(string), typeof(TextBoxAttach));

        public static void SetSuffixText(DependencyObject element, string value)
        {
            element.SetValue(SuffixTextProperty, value);
        }

        public static string GetSuffixText(DependencyObject element)
        {
            return (string)element.GetValue(SuffixTextProperty);
        }
    }
}
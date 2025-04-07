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

        public static readonly DependencyProperty TitlePlacementProperty =
          DependencyProperty.RegisterAttached("TitlePlacement", typeof(Dock), typeof(TextBoxAttach));

        public static Dock GetTitlePlacement(DependencyObject target)
        {
            return (Dock)target.GetValue(TitlePlacementProperty);
        }

        public static void SetTitlePlacement(DependencyObject target, Dock value)
        {
            target.SetValue(TitlePlacementProperty, value);
        }

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

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolder, (sender, e) =>
            {
                string newValue = e.NewValue as string;
                if (sender.IsPlaceHolderObject() && string.IsNullOrEmpty(newValue))
                {
                    SetPlaceHolderVisibility(sender, Visibility.Collapsed);
                }
                var type = sender.GetType();
                if (type == typeof(ComboBox))
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox.IsLoaded)
                    {
                        InvokePlaceHolderChanged(comboBox);
                    }
                    else
                    {
                        comboBox.Loaded += ComboBox_Loaded;
                    }
                }
                else if (type == typeof(TextBox))
                {
                    var textBox = sender as TextBox;
                    textBox.TextChanged -= TextBox_TextChanged;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        UpdateHolderVisibility(textBox, textBox.Text);
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                }
                else if (type == typeof(PasswordBox))
                {
                    var passwordBox = sender as PasswordBox;
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
            ComboBox comboBox = sender as ComboBox;
            comboBox.Loaded -= ComboBox_Loaded;
            InvokePlaceHolderChanged(comboBox);
        }

        private static void InvokePlaceHolderChanged(ComboBox comboBox)
        {
            if (comboBox.Template?.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
            {
                textBox.TextChanged -= ComboBoxTextBox_TextChanged;
                comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                if (!string.IsNullOrEmpty(GetPlaceHolder(comboBox)))
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
            ComboBox comboBox = sender as ComboBox;
            if (!comboBox.IsEditable)
            {
                UpdateHolderVisibility(comboBox, comboBox.SelectedItem?.ToString());
            }
        }

        private static void ComboBoxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.TemplatedParent is ComboBox comboBox && comboBox.IsEditable)
            {
                UpdateHolderVisibility(comboBox, textBox.Text);
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateHolderVisibility(textBox, textBox.Text);
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            UpdateHolderVisibility(passwordBox, passwordBox.Password);
        }

        private static void UpdateHolderVisibility(DependencyObject target, string value)
        {
            if (target != null)
            {
                SetPlaceHolderVisibility(target, string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public static string GetPlaceHolder(DependencyObject target)
        {
            return (string)target.GetValue(PlaceHolderProperty);
        }

        public static void SetPlaceHolder(DependencyObject target, string value)
        {
            target.SetValue(PlaceHolderProperty, value);
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
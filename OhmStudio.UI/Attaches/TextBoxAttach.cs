using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OhmStudio.UI.Controls;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Attaches
{
    public class TextBoxAttach
    {
        public const string PlaceHolder = null;
        public const double PlaceHolderOpacity = 0.6;
        public const bool PlaceHolderIsHitTestVisible = false;

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
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(object), typeof(TextBoxAttach), new PropertyMetadata(PlaceHolder, (sender, e) =>
            {
                var newValue = e.NewValue;
                if (sender.IsPlaceHolderObject() && CheckIsEmpty(newValue))
                {
                    SetPlaceHolderVisibility(sender, Visibility.Collapsed);
                }
                var type = sender.GetType();
                if (type == typeof(ComboBox))
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox.IsLoaded)
                    {
                        InvokeChanged(comboBox);
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
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(textBox, textBox.Text);
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                }
                else if (type == typeof(PasswordBox))
                {
                    var passwordBox = sender as PasswordBox;
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    if (!CheckIsEmpty(newValue))
                    {
                        UpdateHolderVisibility(passwordBox, passwordBox.Password);
                        passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    }
                }
                else if (sender is ITextChanged textChanged)
                {
                    textChanged.TextChanged -= ITextChanged;
                    if (!CheckIsEmpty(newValue))
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

        private static bool CheckIsEmpty(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            else if (obj is string value)
            {
                return string.IsNullOrEmpty(value);
            }
            return false;
        }

        private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Loaded -= ComboBox_Loaded;
            InvokeChanged(comboBox);
        }

        static void InvokeChanged(ComboBox comboBox)
        {
            comboBox.Dispatcher.InvokeAsync(() =>
            {
                if (comboBox.Template?.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
                {
                    textBox.TextChanged -= ComboBoxTextBox_TextChanged;
                    comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                    if (!CheckIsEmpty(GetPlaceHolder(comboBox)))
                    {
                        UpdateHolderVisibility(comboBox, comboBox.IsEditable ? textBox.Text : comboBox.SelectedItem?.ToString());
                        textBox.TextChanged += ComboBoxTextBox_TextChanged;
                        comboBox.SelectionChanged += ComboBox_SelectionChanged;
                    }
                }
            }, DispatcherPriority.Render);
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

        public static readonly DependencyProperty IsShowValidationErrorInfoProperty =
            DependencyProperty.RegisterAttached("IsShowValidationErrorInfo", typeof(bool), typeof(TextBoxAttach), new PropertyMetadata(true));
        public static bool GetIsShowValidationErrorInfo(DependencyObject target)
        {
            return (bool)target.GetValue(IsShowValidationErrorInfoProperty);
        }

        public static void SetIsShowValidationErrorInfo(DependencyObject target, bool value)
        {
            target.SetValue(IsShowValidationErrorInfoProperty, value);
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
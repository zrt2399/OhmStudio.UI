using System;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// PasswordBoxControl.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordBoxControl : UserControl
    {
        public PasswordBoxControl()
        {
            InitializeComponent();
            GotFocus += (sender, e) =>
            {
                if (!txtPassword.IsKeyboardFocusWithin && txtPassword.Visibility == Visibility.Visible)
                {
                    txtPassword.Focus();
                }
                else if (!txtTextBox.IsKeyboardFocusWithin && txtTextBox.Visibility == Visibility.Visible)
                {
                    txtTextBox.Focus();
                }
            };
        }

        /// <summary>
        /// 控制PasswordBoxControl显示或者隐藏CheckBox，来控制是否可以显示和隐藏密码。
        /// </summary>
        public Visibility CanShowPassword
        {
            get => (Visibility)GetValue(CanShowPasswordProperty);
            set => SetValue(CanShowPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanShowPasswordVisibility.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty CanShowPasswordProperty =
            DependencyProperty.Register("CanShowPasswordVisibility", typeof(Visibility), typeof(PasswordBoxControl), new PropertyMetadata(Visibility.Visible, (sender, e) =>
            {
                if ((Visibility)e.NewValue != Visibility.Visible)
                {
                    (sender as PasswordBoxControl).IsChecked = false;
                }
            }));

        /// <summary>
        /// 控制TextBox显示或者隐藏----TextBox来显示明文。
        /// </summary>
        public Visibility TbVisibility
        {
            get => (Visibility)GetValue(TbVisibilityProperty);
            set => SetValue(TbVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for TbVisibility.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty TbVisibilityProperty =
            DependencyProperty.Register("TbVisibility", typeof(Visibility), typeof(PasswordBoxControl), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 控制PassworBox显示或者隐藏----PasswordBox控件来显密文。
        /// </summary>
        public Visibility PwVisibility
        {
            get => (Visibility)GetValue(PwVisibilityProperty);
            set => SetValue(PwVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for PwVisibility.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty PwVisibilityProperty =
            DependencyProperty.Register("PwVisibility", typeof(Visibility), typeof(PasswordBoxControl));

        /// <summary>
        /// 和"眼睛"进行绑定。
        /// </summary>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        // Using a DependencyProperty as the backing store for Check.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(PasswordBoxControl), new PropertyMetadata((sender, e) =>
            {
                PasswordBoxControl passwordBoxControl = sender as PasswordBoxControl;
                if ((bool)e.NewValue)
                {
                    passwordBoxControl.TbVisibility = Visibility.Visible;
                    passwordBoxControl.PwVisibility = Visibility.Collapsed;
                }
                else
                {
                    passwordBoxControl.TbVisibility = Visibility.Collapsed;
                    passwordBoxControl.PwVisibility = Visibility.Visible;
                }
            }));

        /// <summary>
        /// 点击图标"x",使密码框清空。
        /// </summary>
        public bool IsCleared
        {
            get => (bool)GetValue(IsClearedProperty);
            set => SetValue(IsClearedProperty, value);
        }
        public static readonly DependencyProperty IsClearedProperty =
            DependencyProperty.Register("IsCleared", typeof(bool), typeof(PasswordBoxControl), new PropertyMetadata((sender, e) =>
            {
                var passwordBoxControl = sender as PasswordBoxControl;
                passwordBoxControl.Password = string.Empty;
            }));

        /// <summary>
        /// 控制显示符号"x"。
        /// </summary>
        public Visibility ClearVisibility
        {
            get => (Visibility)GetValue(ClearVisibilityProperty);
            set => SetValue(ClearVisibilityProperty, value);
        }
        public static readonly DependencyProperty ClearVisibilityProperty =
            DependencyProperty.Register("ClearVisibility", typeof(Visibility), typeof(PasswordBoxControl), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 密码。
        /// </summary>
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxControl), new PropertyMetadata(string.Empty, (sender, e) =>
            {
                var passwordBox = sender as PasswordBoxControl;
                //根据密码框是否有内容来显示符号"x"
                passwordBox.ClearVisibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Collapsed : Visibility.Visible;
            }));
    }

    /// <summary>
    /// 为PasswordBox控件的Password增加绑定功能。
    /// </summary>
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordBoxHelper));

        public static void SetAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject obj, bool value)
        {
            obj.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
            {
                return;
            }
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
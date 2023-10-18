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
            GotFocus += PasswordBoxControl_GotFocus;
        }

        private void PasswordBoxControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtPassword.IsKeyboardFocusWithin && txtPassword.Visibility == Visibility.Visible)
            {
                txtPassword.Focus();
                e.Handled = true;
            }
            else if (!txtTextBox.IsKeyboardFocusWithin && txtTextBox.Visibility == Visibility.Visible)
            {
                txtTextBox.Focus();
                e.Handled = true;
            }
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
}
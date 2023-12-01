using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// PasswordTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordTextBox : UserControl
    {
        public PasswordTextBox()
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
        /// 控制PasswordTextBox显示或者隐藏CheckBox，来控制是否可以显示和隐藏密码。
        /// </summary>
        public Visibility CanShowPassword
        {
            get => (Visibility)GetValue(CanShowPasswordProperty);
            set => SetValue(CanShowPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanShowPasswordVisibility.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty CanShowPasswordProperty =
            DependencyProperty.Register("CanShowPasswordVisibility", typeof(Visibility), typeof(PasswordTextBox), new PropertyMetadata(Visibility.Visible, (sender, e) =>
            {
                if ((Visibility)e.NewValue != Visibility.Visible && sender is PasswordTextBox passwordTextBox)
                {
                    passwordTextBox.IsChecked = false;
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
            DependencyProperty.Register("TbVisibility", typeof(Visibility), typeof(PasswordTextBox), new PropertyMetadata(Visibility.Collapsed));

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
            DependencyProperty.Register("PwVisibility", typeof(Visibility), typeof(PasswordTextBox));

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
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(PasswordTextBox), new PropertyMetadata((sender, e) =>
            {
                if (sender is PasswordTextBox passwordTextBox)
                {
                    if ((bool)e.NewValue)
                    {
                        passwordTextBox.TbVisibility = Visibility.Visible;
                        passwordTextBox.PwVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        passwordTextBox.TbVisibility = Visibility.Collapsed;
                        passwordTextBox.PwVisibility = Visibility.Visible;
                    }
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
            DependencyProperty.Register("IsCleared", typeof(bool), typeof(PasswordTextBox), new PropertyMetadata((sender, e) =>
            {
                if (sender is PasswordTextBox passwordTextBox)
                {
                    passwordTextBox.Password = string.Empty;
                }
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
            DependencyProperty.Register("ClearVisibility", typeof(Visibility), typeof(PasswordTextBox), new PropertyMetadata(Visibility.Collapsed));

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
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordTextBox), new PropertyMetadata(string.Empty, (sender, e) =>
            {
                //根据密码框是否有内容来显示符号"x"
                if (sender is PasswordTextBox passwordTextBox)
                {
                    passwordTextBox.ClearVisibility = string.IsNullOrEmpty(passwordTextBox.Password) ? Visibility.Collapsed : Visibility.Visible;
                }
            }));
    }
}
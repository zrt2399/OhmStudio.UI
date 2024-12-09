using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    /// <summary>
    /// The types of window system button.
    /// </summary>
    public enum WindowSystemButtonType
    {
        /// <summary>
        /// The menu button.
        /// </summary>
        Menu,
        /// <summary>
        /// The minimize button.
        /// </summary>
        Minimize,
        /// <summary>
        /// The maximize/restore button.
        /// </summary>
        Maximize,
        /// <summary>
        /// The restore button.
        /// </summary>
        Restore,
        /// <summary>
        /// The close button.
        /// </summary>
        Close
    }

    /// <summary>
    /// A window system button. Used to minimize, maximize, restore, and close Windows.
    /// </summary>
    public class WindowSystemButton : ButtonBase
    {
        private Popup PART_Popup;
        /// <summary>
        /// The static class constructor of the <see cref="WindowSystemButton"/>.
        /// </summary>
        static WindowSystemButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowSystemButton), new FrameworkPropertyMetadata(typeof(WindowSystemButton)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSystemButton"/> class.
        /// </summary>
        public WindowSystemButton()
        {
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(WindowSystemButton), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(nameof(ButtonType), typeof(WindowSystemButtonType),
                typeof(WindowSystemButton), new FrameworkPropertyMetadata(WindowSystemButtonType.Menu));

        /// <summary>
        /// The button type of <see cref="WindowSystemButton"/>.
        /// </summary>
        public WindowSystemButtonType ButtonType
        {
            get => (WindowSystemButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(WindowSystemButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (PART_Popup != null)
            {
                PART_Popup.PreviewMouseUp -= PART_Popup_PreviewMouseUp;
            }
            base.OnApplyTemplate();
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
            PART_Popup.PreviewMouseUp += PART_Popup_PreviewMouseUp;
        }

        private void PART_Popup_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
        }

        /// <summary>
        /// 重写按钮点击事件。
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            if (ButtonType == WindowSystemButtonType.Menu)
            {
                if (IsMouseOver)
                {
                    IsDropDownOpen = true;
                }
                return;
            }

            if (Window.GetWindow(this) is Window window)
            {
                switch (ButtonType)
                {
                    case WindowSystemButtonType.Minimize:
                        SystemCommands.MinimizeWindow(window);
                        break;

                    case WindowSystemButtonType.Maximize:
                        SystemCommands.MaximizeWindow(window);
                        break;

                    case WindowSystemButtonType.Restore:
                        SystemCommands.RestoreWindow(window);
                        break;

                    case WindowSystemButtonType.Close:
                        window.Close();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
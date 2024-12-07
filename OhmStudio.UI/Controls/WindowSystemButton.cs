﻿using System.Windows;
using System.Windows.Controls.Primitives;
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
        /// Identifies the <see cref="WindowSystemButtonType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowSystemButtonTypeProperty =
            DependencyProperty.Register(nameof(WindowSystemButtonType), typeof(WindowSystemButtonType),
                typeof(WindowSystemButton), new FrameworkPropertyMetadata(WindowSystemButtonType.Menu));

        /// <summary>
        /// The button type of <see cref="WindowSystemButton"/>.
        /// </summary>
        public WindowSystemButtonType WindowSystemButtonType
        {
            get => (WindowSystemButtonType)GetValue(WindowSystemButtonTypeProperty);
            set => SetValue(WindowSystemButtonTypeProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(WindowSystemButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        /// 重写按钮点击事件。
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            if (WindowSystemButtonType == WindowSystemButtonType.Menu)
            {
                IsDropDownOpen = true;
                return;
            }

            if (Window.GetWindow(this) is Window window)
            {
                switch (WindowSystemButtonType)
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
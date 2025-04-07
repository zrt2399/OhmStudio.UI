using System;
using System.Windows;
using System.Windows.Input;

namespace OhmStudio.UI.Attaches
{
    [Flags]
    public enum MouseClickMode
    {
        None = 0,
        Left = 1,
        Middle = 1 << 1,
        Right = 1 << 2,
        XButton1 = 1 << 3,
        XButton2 = 1 << 4
    }

    public class MouseDownAttach
    {
        public static readonly DependencyProperty IgnoreElementProperty =
            DependencyProperty.RegisterAttached("IgnoreElement", typeof(string),
                typeof(MouseDownAttach), new PropertyMetadata(string.Empty));

        public static string GetIgnoreElement(DependencyObject element)
        {
            return (string)element.GetValue(IgnoreElementProperty);
        }

        public static void SetIgnoreElement(DependencyObject element, string value)
        {
            element.SetValue(IgnoreElementProperty, value);
        }

        public static readonly DependencyProperty MouseClickModeProperty =
            DependencyProperty.RegisterAttached("MouseClickMode", typeof(MouseClickMode),
                typeof(MouseDownAttach), new PropertyMetadata(MouseClickMode.Left | MouseClickMode.Right));

        public static MouseClickMode GetMouseClickMode(DependencyObject element)
        {
            return (MouseClickMode)element.GetValue(MouseClickModeProperty);
        }

        public static void SetMouseClickMode(DependencyObject element, MouseClickMode value)
        {
            element.SetValue(MouseClickModeProperty, value);
        }

        public static readonly DependencyProperty MouseClickCountProperty =
            DependencyProperty.RegisterAttached("MouseClickCount", typeof(int),
                typeof(MouseDownAttach), new PropertyMetadata(1));

        public static int GetMouseClickCount(DependencyObject element)
        {
            return (int)element.GetValue(MouseClickCountProperty);
        }

        public static void SetMouseClickCount(DependencyObject element, int value)
        {
            element.SetValue(MouseClickCountProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
                typeof(MouseDownAttach), new FrameworkPropertyMetadata(OnCommandChanged));

        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not UIElement uIElement)
            {
                return;
            }

            uIElement.PreviewMouseDown -= UIElement_PreviewMouseDown;

            if (e.NewValue != null)
            {
                uIElement.PreviewMouseDown += UIElement_PreviewMouseDown;
            }
        }

        private static void UIElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uIElement && e.ClickCount == GetMouseClickCount(uIElement))
            {
                var ignoreElement = GetIgnoreElement(uIElement);
                if (!string.IsNullOrEmpty(ignoreElement))
                {
                    var types = ignoreElement.Split(',', '|');
                    foreach (var item in types)
                    {
                        if (e.OriginalSource.GetType().Name == item)
                        {
                            return;
                        }
                    }
                }
                var command = GetCommand(uIElement);
                var commandParameter = GetCommandParameter(uIElement);
                var mouseClickMode = GetMouseClickMode(uIElement);
                if ((mouseClickMode & MouseClickMode.Left) != 0 && e.ChangedButton == MouseButton.Left)
                {
                    command?.Execute(commandParameter);
                }
                if ((mouseClickMode & MouseClickMode.Right) != 0 && e.ChangedButton == MouseButton.Right)
                {
                    command?.Execute(commandParameter);
                }
                if ((mouseClickMode & MouseClickMode.Middle) != 0 && e.ChangedButton == MouseButton.Middle)
                {
                    command?.Execute(commandParameter);
                }
                if ((mouseClickMode & MouseClickMode.XButton1) != 0 && e.ChangedButton == MouseButton.XButton1)
                {
                    command?.Execute(commandParameter);
                }
                if ((mouseClickMode & MouseClickMode.XButton2) != 0 && e.ChangedButton == MouseButton.XButton2)
                {
                    command?.Execute(commandParameter);
                }
            }
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDownAttach));

        public static object GetCommandParameter(DependencyObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }
    }
}
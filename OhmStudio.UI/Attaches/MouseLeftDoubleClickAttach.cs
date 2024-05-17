using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OhmStudio.UI.Attaches
{
    public class MouseLeftDoubleClickAttach
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
                typeof(MouseLeftDoubleClickAttach), new FrameworkPropertyMetadata(OnCommandChanged));

        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not Control control)
            {
                return;
            }

            control.MouseDoubleClick -= OnMouseDoubleClick;

            if (e.NewValue != null)
            {
                control.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Control control)
            {
                var command = GetCommand(control);
                command?.Execute(GetCommandParameter(control));

                e.Handled = true;
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
           DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseLeftDoubleClickAttach));

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
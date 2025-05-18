using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Messaging
{
    public enum IconType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public static class MessageTip
    {
        private const int Delay = 2000;

        public static int GlobalDelay { get; set; } = 0;

        public static CornerRadius TipCornerRadius { get; set; } = new CornerRadius(4);

        public static void Show(string message, int delay = Delay) => Show(message, IconType.Information, delay);

        public static void ShowSuccess(string message, int delay = Delay) => Show(message, IconType.Success, delay);

        public static void ShowWarning(string message, int delay = Delay) => Show(message, IconType.Warning, delay);

        public static void ShowError(string message, int delay = Delay) => Show(message, IconType.Error, delay);

        public static async void Show(string message, IconType iconType, int delay = Delay)
        {
            if (Application.Current is not Application application || application.Dispatcher == null)
            {
                return;
            }
            if (application.Dispatcher.CheckAccess())
            {
                if (GlobalDelay > 0)
                {
                    delay = GlobalDelay;
                }

                var borderBrush = iconType switch
                {
                    IconType.Information => "#8C8C8C".ToSolidColorBrush(),
                    IconType.Success => "#6EBE28".ToSolidColorBrush(),
                    IconType.Warning => "#DC9B28".ToSolidColorBrush(),
                    _ => "#E65050".ToSolidColorBrush()
                };

                Grid gridContent = new Grid();
                gridContent.Margin = new Thickness(4);
                gridContent.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                gridContent.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock textBlock = new TextBlock { TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, Text = message };

                if (!string.IsNullOrEmpty(textBlock.Text))
                {
                    textBlock.Margin = new Thickness(4, 0, 0, 0);
                }

                textBlock.SetValue(Grid.ColumnProperty, 1);
                gridContent.Children.Add(new TipIcon() { IconType = iconType, Padding = new Thickness(1) });
                gridContent.Children.Add(textBlock);

                Border border = new Border();
                border.Child = gridContent;
                border.Background = "#FAFAFA".ToSolidColorBrush();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = borderBrush;
                border.CornerRadius = TipCornerRadius;

                Border borderEffect = new Border();
                borderEffect.Background = border.Background;
                borderEffect.CornerRadius = border.CornerRadius;
                borderEffect.BorderThickness = border.BorderThickness;
                borderEffect.BorderBrush = Brushes.Transparent;
                borderEffect.Effect = new DropShadowEffect() { ShadowDepth = 0, Color = borderBrush.Color, BlurRadius = 8 };

                Grid grid = new Grid();
                grid.Margin = new Thickness(4);
                grid.Children.Add(borderEffect);
                grid.Children.Add(border);

                Popup popup = new Popup();
                popup.AllowsTransparency = true;
                popup.Child = grid;
                popup.MaxWidth = SystemParameters.WorkArea.Width;
                popup.StaysOpen = true;
                popup.Placement = PlacementMode.Mouse;
                popup.IsOpen = true;
                await Task.Delay(delay);
                popup.IsOpen = false;
            }
            else
            {
                application.Dispatcher.Invoke(() => Show(message, iconType, delay));
            }
        }
    }
}
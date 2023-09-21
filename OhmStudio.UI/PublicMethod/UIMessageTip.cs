using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point; 

namespace OhmStudio.UI.PublicMethod
{
    public enum UIStyle
    {
        Info,
        OK,
        Warning,
        Error
    }

    public static class UIMessageTip
    {
        const int Delay = 2000;
        public static void Show(string message, int delay = Delay) => Show(message, UIStyle.Info, delay);

        public static void ShowOk(string message, int delay = Delay) => Show(message, UIStyle.OK, delay);

        public static void ShowWarning(string message, int delay = Delay) => Show(message, UIStyle.Warning, delay);

        public static void ShowError(string message, int delay = Delay) => Show(message, UIStyle.Error, delay);

        public static void Show(string message, UIStyle uIStyle, int delay = Delay)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                Popup popup = new Popup();
                popup.AllowsTransparency = true;
                Border borderEffect = new Border();
                borderEffect.Background = "#FAFAFA".ToSolidColorBrush();
                var color = uIStyle switch
                {
                    UIStyle.Info => "#8C8C8C".ToSolidColorBrush(),
                    UIStyle.OK => "#6EBE28".ToSolidColorBrush(),
                    UIStyle.Warning => "#DC9B28".ToSolidColorBrush(),
                    _ => "#E65050".ToSolidColorBrush()
                };
                borderEffect.CornerRadius = new CornerRadius(4);
                borderEffect.Margin = new Thickness(4);
                borderEffect.Effect = new DropShadowEffect() { ShadowDepth = 0, Color = color.Color };

                Grid grid = new Grid();
                Grid gridContent = new Grid();
                gridContent.ColumnDefinitions.Add(new ColumnDefinition());
                gridContent.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Text = message;
                //textBlock.FontSize = 13;
                if (!string.IsNullOrEmpty(textBlock.Text))
                {
                    textBlock.Margin = new Thickness(4, 0, 0, 0);
                }
                var image = new System.Windows.Controls.Image();
                image.Source = PresetsResources.Icons[(int)uIStyle];
                textBlock.SetValue(Grid.ColumnProperty, 1);
                gridContent.Children.Add(image);
                gridContent.Children.Add(textBlock);

                Border border = new Border();
                border.Child = gridContent;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new System.Windows.Media.SolidColorBrush(color.Color);
                border.Padding = new Thickness(4);
                border.Margin = new Thickness(3.5);
                border.CornerRadius = new CornerRadius(4);

                grid.Children.Add(borderEffect);
                grid.Children.Add(border);
                popup.Child = grid;
                popup.StaysOpen = true;
                popup.Placement = PlacementMode.Mouse;
                popup.IsOpen = true;
                await Task.Delay(delay);
                popup.IsOpen = false;
            });
        }
    }

    /// <summary>
    /// 预置资源。
    /// </summary>
    public static class PresetsResources
    {
        /// <summary>
        /// 绿
        /// </summary>
        public static readonly Color Green = ColorTranslator.FromHtml("#6EBE28");

        /// <summary>
        /// 红
        /// </summary>
        public static readonly Color Red = ColorTranslator.FromHtml("#E65050");

        /// <summary>
        /// 灰
        /// </summary>
        public static readonly Color Gray = ColorTranslator.FromHtml("#8C8C8C");

        /// <summary>
        /// 橙
        /// </summary>
        public static readonly Color Orange = ColorTranslator.FromHtml("#DC9B28");

        /// <summary>
        /// 浅绿
        /// </summary>
        public static readonly Color LightGreen = ColorTranslator.FromHtml("#EFF8E8");

        /// <summary>
        /// 浅红
        /// </summary>
        public static readonly Color LightRed = ColorTranslator.FromHtml("#FBEEEE");

        /// <summary>
        /// 浅灰
        /// </summary>
        public static readonly Color LightGray = ColorTranslator.FromHtml("#F5F5F5");

        /// <summary>
        /// 浅橙
        /// </summary>
        public static readonly Color LightOrange = ColorTranslator.FromHtml("#FBF5E9");

        public static readonly Color[,] Colors =
        {
                    //边框色、背景色、阴影色
             /*灰*/ {Gray, LightGray, ColorTranslator.FromHtml("#6E000000")},
             /*绿*/ {Green, LightGreen, ColorTranslator.FromHtml("#96009600")},
             /*橙*/ {Orange, LightOrange, ColorTranslator.FromHtml("#96FA6400")},
             /*红*/ {Red, LightRed, ColorTranslator.FromHtml("#8CFF1E1E")}
        };

        //CreateIcon依赖Colors，所以需在Colors后初始化
        public static readonly BitmapImage[] Icons = { CreateIcon(0), CreateIcon(1), CreateIcon(2), CreateIcon(3) };

        /// <summary>
        /// 创建图标。
        /// </summary>
        /// <param name="index">0=i；1=√；2=！；3=×；</param>
        private static BitmapImage CreateIcon(int index)
        {
            if (index < 0 || index > 3)
            {
                return null;
            }
            Graphics graphics = null;
            Pen pen = null;
            Brush brush = null;
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(24, 24);
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Color color = Colors[index, 0];
                if (index == 0)//i
                {
                    brush = new SolidBrush(Color.FromArgb(103, 148, 186));
                    graphics.FillEllipse(brush, 3, 3, 18, 18);

                    pen = new Pen(Colors[index, 1], 2);
                    graphics.DrawLine(pen, new Point(12, 6), new Point(12, 8));
                    graphics.DrawLine(pen, new Point(12, 10), new Point(12, 18));
                }
                else if (index == 1)//√
                {
                    pen = new Pen(color, 4);
                    graphics.DrawLines(pen, new[] { new Point(3, 11), new Point(10, 18), new Point(20, 5) });
                }
                else if (index == 2)//！
                {
                    var points = new[] { new Point(12, 3), new Point(3, 20), new Point(21, 20) };
                    pen = new Pen(color, 2) { LineJoin = LineJoin.Bevel };
                    graphics.DrawPolygon(pen, points);

                    brush = new SolidBrush(color);
                    graphics.FillPolygon(brush, points);

                    pen.Color = Colors[index, 1];
                    graphics.DrawLine(pen, new Point(12, 8), new Point(12, 15));
                    graphics.DrawLine(pen, new Point(12, 17), new Point(12, 19));
                }
                else//×
                {
                    pen = new Pen(color, 4);
                    graphics.DrawLine(pen, 5, 5, 19, 19);
                    graphics.DrawLine(pen, 5, 19, 19, 5);
                }
                //bmp.Save("C:\\Users\\Xtone\\Desktop\\1.bmp");
                return bitmap.ToBitmapImage(ImageFormat.Png);
            }
            catch
            {
                bitmap?.Dispose();
                throw;
            }
            finally
            {
                pen?.Dispose();
                brush?.Dispose();
                graphics?.Dispose();
            }
        }
    }
}
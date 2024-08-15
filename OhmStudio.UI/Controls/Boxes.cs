using System.Windows;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    internal static class ObjectBoxes
    {
        public static readonly object NullBox = null;
        public static readonly object EmptyBox = string.Empty;
        public static readonly object ThicknessBox = default(Thickness);
        public static readonly object TransparentBox = Brushes.Transparent;
    }
}
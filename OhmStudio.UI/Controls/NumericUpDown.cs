using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    public class NumericUpDown : Control
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }
    }
} 
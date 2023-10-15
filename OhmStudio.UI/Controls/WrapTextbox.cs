using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    public class WrapTextBox : WrapPanel
    {
        public WrapTextBox()
        {
            this.Children.Add(textBlock);
            this.Children.Add(textBox);
        }

        TextBlock textBlock;
        TextBox  textBox;

     
    }
}
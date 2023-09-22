using System.Windows.Automation.Peers;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Automation.Peers
{
    public class CustomChromeWindowAutomationPeer : WindowAutomationPeer
    {
        public CustomChromeWindowAutomationPeer(CustomChromeWindow owner) : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(CustomChromeWindow);
        }
    }
}
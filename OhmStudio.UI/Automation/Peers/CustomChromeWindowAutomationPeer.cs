using System.Windows.Automation.Peers;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Automation.Peers
{
    public class ChromeWindowAutomationPeer : WindowAutomationPeer
    {
        public ChromeWindowAutomationPeer(ChromeWindow owner) : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(ChromeWindow);
        }
    }
}
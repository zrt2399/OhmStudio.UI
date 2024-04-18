using Microsoft.Win32;

namespace OhmStudio.UI.Helpers
{
    public class SystemHelper
    {
        private const string SkinTypeRegistryKeyName = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        private const string SkinTypeRegistryValueName = "AppsUseLightTheme";

        public static bool DetermineIfInLightThemeMode
        {
            get
            {
                object value = Registry.GetValue(SkinTypeRegistryKeyName, SkinTypeRegistryValueName, "0");
                if (value != null)
                {
                    return value.ToString() != "0";
                }

                return false;
            }
        }
    } 
}
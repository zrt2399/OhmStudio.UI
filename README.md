# OhmStudio.UI

#### Introduce
WPF UI Library

AvalonDock UI Library

### .Net Version
|.Net Version   | Status  |
|  ----  | ----  |
| net462  | ✅ |
| net5.0-windows  | ✅ |

# 💡 Install
Nuget search to download OhmStudio.UI. Please use the latest version and I will update this package from time to time to fix any bugs that may occur during testing

# 🚀 Quick Start
``` xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            WPF control and AvalonDock theme, 2022 can be replaced with 2019, DarkTheme can be replaced with LightTheme and BlueTheme
            <ResourceDictionary Source="/OhmStudio.UI;Component/Themes/VisualStudio2022/DarkTheme.xaml" />

            Custom control styles and WPF native controls, For example SearchBar, CheckComboBox, CircularProgressBar, etc...
            <ResourceDictionary Source="/OhmStudio.UI;Component/Styles/VisualStudio.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
#### Contribution

- [OriginalAuthor：Wenveo](https://www.bilibili.com/video/BV1yW4y1N7Zq/?spm_id_from=333.999.0.0)
- [AakStudio.Shell.UI](https://github.com/Wenveo/AakStudio.Shell.UI)
- [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock)

The original author is no longer maintaining the change library. I am organizing and updating this package
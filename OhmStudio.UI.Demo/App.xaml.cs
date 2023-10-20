using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace OhmStudio.UI.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        public static bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
 
        public static readonly string DocumentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Name);
    }
}
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using OhmStudio.UI.Messaging;

namespace OhmStudio.UI.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;

        public static readonly string DocumentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Name);

        public static readonly string FrameworkDisplayName = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkDisplayName; 

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;
                var message = "在工作线程发生未捕获的异常：\r\n";
                AlertDialog.Show(message + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            DispatcherUnhandledException += (sender, e) =>
            {
                e.Handled = true;
                Exception ex = e.Exception;
                var message = "在主线程发生未捕获的异常：\r\n";
                AlertDialog.Show(message + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                var message = $"{e.Exception.Message}\r\n{e.Exception.InnerException}";
                AlertDialog.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
#endif
            base.OnStartup(e);
        }
    }
}
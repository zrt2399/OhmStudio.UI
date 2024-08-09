using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OhmStudio.UI.Demo.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<MainViewModel>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static IServiceProvider _serviceProvider;

        public static MainViewModel MainViewModel => GetInstance<MainViewModel>();

        public static T GetInstance<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            return _serviceProvider.GetServices<T>();
        }
    }
}

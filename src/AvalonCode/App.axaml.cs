using AvalonCode.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaReactorUI;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvalonCode
{
    public class App : Application
    {
        static App()
        {
            var services = new ServiceCollection();
            services.AddAvalonCodeServices();
            Services = services.BuildServiceProvider();
        }

        public override void Initialize()
        {
            MSBuildLocator.RegisterDefaults();

            AvaloniaXamlLoader.Load(this);
        }

        public static IServiceProvider Services { get; private set; }

        public override void OnFrameworkInitializationCompleted()
        {
            RxApplicationBuilder<Shell.MainWindow>
                .Create(this)
                .Run();

            base.OnFrameworkInitializationCompleted();
        }
    }
}

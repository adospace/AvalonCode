using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace AvalonCode
{
    class Program
    {
        //C:\Source\github\reactorui-avalonia\src\AvaloniaReactorUI.HotReloader\bin\Debug\net5.0\AvaloniaReactorUI.HotReloader.exe C:\Source\github\reactorui-avalonia\src\AvaloniaReactorUI.DemoApp C:\Source\github\reactorui-avalonia\src\AvaloniaReactorUI.DemoApp\bin\Debug\net5.0\AvaloniaReactorUI.DemoApp.dll

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}

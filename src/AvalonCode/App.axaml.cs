using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaReactorUI;

namespace AvalonCode
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            RxApplicationBuilder<Shell.MainWindow>
                .Create(this)
                .Run();

            base.OnFrameworkInitializationCompleted();
        }
    }
}

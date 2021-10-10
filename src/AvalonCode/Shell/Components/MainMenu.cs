using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class MainMenu : RxComponent
    {
        public override VisualNode Render()
        {
            return new RxMenu()
            {
                new RxMenuItem("File")
                {
                    new RxMenuItem("Exit")
                        .OnClick(OnExitApplication)
                }
            };
        }

        private void OnExitApplication()
        {
            ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).Shutdown();
        }
    }
}

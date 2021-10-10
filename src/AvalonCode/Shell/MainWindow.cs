using AvalonCode.Shell.Components;
using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell
{
    public class MainWindowState : IState
    { 
    
    }

    public class MainWindow : RxComponent<MainWindowState>
    {
        public override VisualNode Render() =>
            new RxWindow()
            {
                RenderShell()
            }
            .Title("AvalonCode");

        private VisualNode RenderShell() => new RxGrid
        {
            new MainMenu()
                .GridRow(0),

            RerderBody()
                .GridRow(1),

            new StatusBar()
                .GridRow(2),
        }
        .Rows("Auto,*,Auto");

        private VisualNode RerderBody()
        {
            return new RxGrid
            {
                new SolutionExplorer(),

                new RxGridSplitter()
                    .ResizeBehavior(Avalonia.Controls.GridResizeBehavior.PreviousAndNext)
                    .ResizeDirection(Avalonia.Controls.GridResizeDirection.Columns)
                    .GridColumn(1),

                new CodeTabs()
                    .GridColumn(2)
            }
            .Columns("300,Auto,*");
        }
    }
}

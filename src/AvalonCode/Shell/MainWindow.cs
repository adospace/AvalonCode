using AvalonCode.Services;
using AvalonCode.Services.Models;
using AvalonCode.Shell.Components;
using Avalonia.Controls;
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

    public class ApplicationParameters
    { 
        public SolutionInfo? Solution { get; set; }

        public string? StatusMessage { get; set; }

        public List<ILoadedDocumentItem> LoadedDocuments { get; set; } = new();

        public ILoadedDocumentItem? CurrentLoadedDocument { get; set; }
    }

    public class MainWindow : RxComponent<MainWindowState>
    {
        private readonly IParameter<ApplicationParameters> _applicationParameters;

        public MainWindow()
        {
            _applicationParameters = CreateParameter<ApplicationParameters>();
        }

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

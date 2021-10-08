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
            }
            .Title("AvalonCode");
    }
}

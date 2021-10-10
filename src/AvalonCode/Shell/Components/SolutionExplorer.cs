using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class SolutionExplorer : RxComponent
    {
        public override VisualNode Render()
        {
            return new RxTextBlock().Text("Solution Explorer");
        }
    }
}

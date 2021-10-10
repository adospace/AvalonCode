using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class CodeTabs : RxComponent
    {
        public override VisualNode Render()
        {
            return new RxTabControl()
            {
                new RxTabItem("Document1"),
                new RxTabItem("Document2")
            };
        }
    }
}

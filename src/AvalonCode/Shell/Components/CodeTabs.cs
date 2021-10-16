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
                .Items(new[] { "Docu1", "Docu2" })
                .OnRenderItem<RxTabControl, string>(_ => new RxTextBlock(_));
            ;
        }
    }
}

﻿using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class StatusBar : RxComponent
    {
        public override VisualNode Render()
        {
            return new RxTextBlock().Text("StatusBar");
        }
    }
}

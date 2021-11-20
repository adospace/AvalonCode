using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Models
{
    public class SolutionInfo : SolutionItem
    {
        internal SolutionInfo(string name, string filename, MSBuildWorkspace workspace)
            : base(name)
        {
            Filename = filename;
            Workspace = workspace;
        }

        public string Filename { get; }
        public MSBuildWorkspace Workspace { get; }
    }
}

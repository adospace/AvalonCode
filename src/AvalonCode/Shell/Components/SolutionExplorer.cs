using AvaloniaReactorUI;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
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
            var applicationParameters = GetParameter<ApplicationParameters>();

            return new RxGrid()
            {
                new RxTextBlock().Text("Solution Explorer"),

                applicationParameters.Value.Workspace == null ?
                null
                :
                new RxTreeView()
                    .Items(applicationParameters.Value.Workspace.CurrentSolution.Projects)
                    .OnRenderTreeItem<RxTreeView, object>(RenderSolutionItem, _ => GetChildrenOfItem(_))
                    .GridRow(1)
            }
            .Rows("Auto,*");                
        }

        private IEnumerable GetChildrenOfItem(object item)
        {
            if (item is Project project)
            {
                foreach (var childItem in project.Documents)
                {
                    yield return childItem;
                }
            }
        }

        private VisualNode RenderSolutionItem(object item)
        {
            if (item is Project project)
            {
                return RenderProjectItem(project);
            }
            if (item is Document document)
            {
                return RenderProjectItem(document);
            }



            return null;
        }

        private VisualNode RenderProjectItem(Project project)
        {
            return new RxTextBlock(project.Name);
        }

        private VisualNode RenderProjectItem(Document document)
        {
            return new RxTextBlock(document.Name);
        }
    }
}

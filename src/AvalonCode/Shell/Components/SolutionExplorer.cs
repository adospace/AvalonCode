using AvalonCode.Services.Models;
using Avalonia.Media;
using AvaloniaReactorUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    //public class ProjectFolder
    //{
    //    public ProjectFolder(string name, Document[] documents, int index = 0)
    //    {
    //        Name = name;
    //        Documents = documents;
    //        Index = index;
    //    }

    //    public string Name { get; }
    //    public Document[] Documents { get; }
    //    public int Index { get; }

    //}

    public class SolutionExplorer : RxComponent
    {
        public override VisualNode Render()
        {
            var applicationParameters = GetParameter<ApplicationParameters>();

            return new RxGrid()
            {
                new RxTextBlock().Text("Solution Explorer"),

                applicationParameters.Value.Solution == null ?
                null
                :
                new RxTreeView()
                    .Items(applicationParameters.Value.Solution.Children)
                    .OnRenderTreeItem<RxTreeView, SolutionItem>(RenderSolutionItem, _ => _)
                    .GridRow(1)
            }
            .Rows("Auto,*");                
        }

        private VisualNode RenderSolutionItem(SolutionItem item)
        {
            if (item is ProjectItem project)
            {
                return RenderItem(project);
            }
            if (item is DocumentItem document)
            {
                return RenderItem(document);
            }
            if (item is SolutionFolderItem solutionFolder)
            {
                return RenderItem(solutionFolder);
            }
            if (item is ProjectFolderItem projectFolder)
            {
                return RenderItem(projectFolder);
            }



            return new RxTextBlock(item.Name);
        }

        private VisualNode RenderItem(ProjectItem project)
        {
            return new RxTextBlock(project.Name);
        }

        private VisualNode RenderItem(DocumentItem document)
        {
            return new RxTextBlock(document.Name)
                .Background(Brushes.Transparent)
                .OnDoubleTapped(()=>OnItemActivated(document));
        }
        private VisualNode RenderItem(SolutionFolderItem solutionFolder)
        {
            return new RxTextBlock(solutionFolder.Name);
        }
        private VisualNode RenderItem(ProjectFolderItem projectFolder)
        {
            return new RxTextBlock(projectFolder.Name);
        }
        private async void OnItemActivated(DocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            var existingDocument = applicationParameters.Value.LoadedDocuments.FirstOrDefault(_ => _.DocumentItem.Id == document.Id);

            if (existingDocument == null)
            {
                applicationParameters.Set(_ => _.StatusMessage = $"Loading {document.Document.FilePath}...");
                existingDocument = await document.Load();
                applicationParameters.Value.LoadedDocuments.Add(existingDocument);
            }

            applicationParameters.Set(_ =>
            {
                _.CurrentLoadedDocument = existingDocument;
                _.CurrentLoadedDocument.LastActivation = DateTime.Now;
                _.StatusMessage = null;
            });
        }

    }
}

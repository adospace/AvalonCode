using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Models
{
    public class ProjectItem : SolutionItem
    {
        public ProjectItem(string name, string fileName, MSBuildWorkspace workspace) : base(name)
        {
            FileName = fileName;
            Workspace = workspace;
        }

        public string FileName { get; }
        public MSBuildWorkspace Workspace { get; }
        public Project? Project { get; private set; }
        public Compilation? Compilation { get; private set; }
        public bool IsLoaded { get; private set; }

        public async Task Load(CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                return;
            }

            var project = Workspace.CurrentSolution.Projects.FirstOrDefault(_=>_.FilePath == FileName);

            if (project == null)
            {
                return;
            }

            static SolutionFolderItem GetOrCreateFolder(SolutionItem parent, string[] folders)
            {
                var folder = parent[folders[0]];
                if (folder == null)
                {
                    parent.AddChild(folder = new SolutionFolderItem(folders[0]));
                }

                if (folders.Length > 1)
                {
                    return GetOrCreateFolder(folder, folders.Skip(1).ToArray());
                }

                return (SolutionFolderItem)folder;
            }

            foreach (var document in project.Documents)
            {
                if (document.Folders.Count == 0)
                {
                    AddChild(new DocumentItem(document));
                }
                else
                {
                    var parentFolder = GetOrCreateFolder(this, document.Folders.ToArray());
                    parentFolder.AddChild(new DocumentItem(document));
                }
            }

            Project = project;
            Compilation = await project.GetCompilationAsync(cancellationToken);

            IsLoaded = true;
        }
    }
}

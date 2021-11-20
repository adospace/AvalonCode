using AvalonCode.Services.Models;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Implementation
{
    internal class SolutionExplorer : ISolutionExplorer
    {
        public SolutionInfo? CurrentSolution { get; private set; }

        public async Task<SolutionInfo> OpenSolution(string solutionFilePath, CancellationToken cancellationToken = default)
        {
            await CloseSolution();

            var workspace = MSBuildWorkspace.Create();

            await workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cancellationToken);
            //applicationParameters.Set(_ => _.StatusMessage = $"Loading {selectedFile}...");
            //var solution = await workspace.OpenSolutionAsync(selectedFile);

            var solutionFile = Microsoft.Build.Construction.SolutionFile.Parse(solutionFilePath);

            var solutionInfo = new SolutionInfo(Path.GetFileNameWithoutExtension(solutionFilePath), solutionFilePath, workspace);

            var solutionItemsDictionary = solutionFile.ProjectsInOrder
                .ToDictionary(_=>_.ProjectGuid, _ =>
                {
                    if (_.ProjectType == Microsoft.Build.Construction.SolutionProjectType.SolutionFolder)
                        return new SolutionFolderItem(_.ProjectName);

                    if (_.ProjectType == Microsoft.Build.Construction.SolutionProjectType.KnownToBeMSBuildFormat)
                        return new ProjectItem(_.ProjectName, _.AbsolutePath, workspace);

                    return (SolutionItem)new UnknownItem(_.ProjectName);
                });

            foreach (var itemInSolution in solutionFile.ProjectsInOrder)
            {
                var solutionItem = solutionItemsDictionary[itemInSolution.ProjectGuid];
                
                if (solutionItem == null) continue;

                if (itemInSolution.ParentProjectGuid == null)
                {
                    solutionInfo.AddChild(solutionItem);
                }
                else
                {
                    var parentSolutionItem = solutionItemsDictionary[itemInSolution.ParentProjectGuid];
                    
                    if (parentSolutionItem == null) continue;

                    parentSolutionItem.AddChild(solutionItem);
                }
            }

            return CurrentSolution = solutionInfo;
        }

        public Task CloseSolution()
        {
            if (CurrentSolution != null)
            {
                CurrentSolution.Workspace.Dispose();
                CurrentSolution = null;
            }

            return Task.CompletedTask;
        }

    }
}

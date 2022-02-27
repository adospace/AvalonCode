using AvalonCode.Services.Models;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Implementation
{
    internal class SolutionExplorer : ISolutionExplorer
    {
        public SolutionHost? CurrentSolution { get; private set; }

        public async Task<SolutionHost> OpenSolution(string solutionFilePath, IEnumerable<Assembly>? additionalAssemblies = null, CancellationToken cancellationToken = default)
        {
            await CloseSolution();

            var workspace = MSBuildWorkspace.Create();

            await workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cancellationToken);

            var solutionFile = Microsoft.Build.Construction.SolutionFile.Parse(solutionFilePath);

            var solutionInfo = new SolutionHost(workspace, additionalAssemblies);

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

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaReactorUI;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class MainMenu : RxComponent
    {
        public override VisualNode Render()
        {
            //return new RxTextBlock("asdasd");
            return new RxMenu()
            {
                new RxMenuItem("File")
                {
                    new RxMenuItem("Open Solution...")
                        .OnClick(OnOpenSolution),


                    new RxMenuItem("Exit")
                        .OnClick(OnExitApplication),
                }
            };
        }

        private async void OnOpenSolution()
        {
            OpenFileDialog dialog = new();
            dialog.Filters.Add(new FileDialogFilter()
            {
                Name = "All Solution Files(*.sln)",
                Extensions = { "sln" }
            });

            var applicationParameters = GetParameter<ApplicationParameters>();
            var applicationLifeTime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            var selectedFiles = await dialog.ShowAsync(applicationLifeTime?.MainWindow ?? throw new InvalidOperationException());

            var selectedFile = selectedFiles.FirstOrDefault();

            if (selectedFile != null)
            {
                AnalyzerManager manager = new(selectedFile);

                //IProjectAnalyzer analyzer = manager.Projects.First().Value;
                AdhocWorkspace workspace = new();

                foreach (var project in manager.Projects)
                {
                    applicationParameters.Set(_ => _.StatusMessage = $"Loading {project.Value.ProjectFile.Path}...");
                    project.Value.AddToWorkspace(workspace);
                }

                applicationParameters.Set(_ => _.Workspace = workspace);
            }
        }

        private void OnExitApplication()
        {
            ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).Shutdown();
        }
    }
}

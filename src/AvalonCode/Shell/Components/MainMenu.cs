using AvalonCode.Services;
using AvalonCode.Services.Models;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaReactorUI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;
using RoslynPad.Editor;
using RoslynPad.Roslyn.QuickInfo;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
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
        protected virtual ImmutableArray<Assembly> CompositionAssemblies => ImmutableArray.Create(
            typeof(App).Assembly, 
            typeof(DeferredQuickInfoContentProvider).Assembly,
            typeof(SnippetInfoService).Assembly);

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

            if (selectedFiles == null)
            {
                return;
            }

            var selectedFile = selectedFiles.FirstOrDefault();

            if (selectedFile != null)
            {
                applicationParameters.Set(_ => _.StatusMessage = $"Loading {selectedFile}...");

                var solutionExplorer = App.Services.GetRequiredService<ISolutionExplorer>();

                var solutionInfo = await solutionExplorer.OpenSolution(selectedFile, CompositionAssemblies); // new[] { typeof(MarkerMargin).Assembly }

                foreach (var project in solutionInfo.Descendents().OfType<ProjectItem>())
                {
                    applicationParameters.Set(_ => _.StatusMessage = $"Loading {project.FileName}...");

                    await project.Load();
                }

                applicationParameters.Set(_ =>
                {
                    _.Solution = solutionInfo;
                    _.StatusMessage = string.Empty;
                });
            }
        }

        private void OnExitApplication()
        {
            ((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).Shutdown();
        }
    }
}

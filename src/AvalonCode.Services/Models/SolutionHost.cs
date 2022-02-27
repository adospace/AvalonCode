using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using RoslynPad.Roslyn.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using AnalyzerReference = Microsoft.CodeAnalysis.Diagnostics.AnalyzerReference;
using AnalyzerFileReference = Microsoft.CodeAnalysis.Diagnostics.AnalyzerFileReference;


namespace AvalonCode.Services.Models
{
    public class SolutionHost : SolutionItem, IRoslynHost
    {
        internal static readonly ImmutableArray<string> PreprocessorSymbols =
            ImmutableArray.CreateRange(new[] { "TRACE", "DEBUG" });

        internal static readonly ImmutableArray<Assembly> DefaultCompositionAssemblies =
            ImmutableArray.Create(
                // Microsoft.CodeAnalysis.Workspaces
                typeof(WorkspacesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Workspaces
                typeof(CSharpWorkspaceResources).Assembly,
                // Microsoft.CodeAnalysis.Features
                typeof(FeaturesResources).Assembly,
                // Microsoft.CodeAnalysis.CSharp.Features
                typeof(CSharpFeaturesResources).Assembly,
                // RoslynPad.Roslyn
                typeof(RoslynHost).Assembly);

        private readonly ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers;
        private readonly IDocumentationProviderService _documentationProviderService;
        private readonly CompositionHost _compositionContext;

        public HostServices HostServices { get; }
        public ParseOptions ParseOptions { get; }
        public ImmutableArray<MetadataReference> DefaultReferences { get; }
        public ImmutableArray<string> DefaultImports { get; }
        public ImmutableArray<string> DisabledDiagnostics { get; }

        internal SolutionHost(MSBuildWorkspace workspace, IEnumerable<Assembly>? additionalAssemblies = null)
            : base(Path.GetFileNameWithoutExtension(workspace.CurrentSolution.FilePath) ?? throw new InvalidOperationException())
        {
            Workspace = workspace;

            var references = RoslynHostReferences.Empty;

            _diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();

            var assemblies = GetDefaultCompositionAssemblies();

            if (additionalAssemblies != null)
            {
                assemblies = assemblies.Concat(additionalAssemblies);
            }

            var partTypes = assemblies
                .SelectMany(x => x.DefinedTypes)
                .Select(x => x.AsType());

            _compositionContext = new ContainerConfiguration()
                .WithParts(partTypes)
                .CreateContainer();

            HostServices = MefHostServices.Create(_compositionContext);

            //_compositionContext.GetExport<RoslynPad.Roslyn.QuickInfo.IQuickInfoProvider>();
            //_compositionContext.GetExport<RoslynPad.Roslyn.QuickSnippetInfoService>();

            ParseOptions = CreateDefaultParseOptions();

            _documentationProviderService = GetService<IDocumentationProviderService>();

            DefaultReferences = references.GetReferences(DocumentationProviderFactory);
            DefaultImports = references.Imports;

            //DisabledDiagnostics = disabledDiagnostics ?? ImmutableArray<string>.Empty;
            DisabledDiagnostics = ImmutableArray<string>.Empty;
            GetService<IDiagnosticService>().DiagnosticsUpdated += OnDiagnosticsUpdated;
        }

        public string? Filename => Workspace.CurrentSolution.FilePath;

        public MSBuildWorkspace Workspace { get; }

        public Func<string, DocumentationProvider> DocumentationProviderFactory => _documentationProviderService.GetDocumentationProvider;

        protected virtual IEnumerable<Assembly> GetDefaultCompositionAssemblies() =>
            DefaultCompositionAssemblies;

        protected virtual ParseOptions CreateDefaultParseOptions() => new CSharpParseOptions(
            preprocessorSymbols: PreprocessorSymbols,
            languageVersion: LanguageVersion.Preview);

        public MetadataReference CreateMetadataReference(string location) => MetadataReference.CreateFromFile(location,
            documentation: _documentationProviderService.GetDocumentationProvider(location));


        public DocumentId AddDocument(DocumentCreationArgs args)
        {
            throw new NotImplementedException();
        }

        public void CloseDocument(DocumentId documentId)
        {
            throw new NotImplementedException();
        }

        public Document? GetDocument(DocumentId documentId)
        {
            return Workspace.CurrentSolution.GetDocument(documentId);
        }

        public TService GetService<TService>() => _compositionContext.GetExport<TService>();


        private void OnDiagnosticsUpdated(object? sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs)
        {
            var documentId = diagnosticsUpdatedArgs.DocumentId;
            if (documentId == null) return;

            if (_diagnosticsUpdatedNotifiers.TryGetValue(documentId, out var notifier))
            {
                if (diagnosticsUpdatedArgs.Kind == DiagnosticsUpdatedKind.DiagnosticsCreated)
                {
                    var remove = diagnosticsUpdatedArgs.Diagnostics.RemoveAll(d => DisabledDiagnostics.Contains(d.Id));
                    if (remove.Length != diagnosticsUpdatedArgs.Diagnostics.Length)
                    {
                        diagnosticsUpdatedArgs = diagnosticsUpdatedArgs.WithDiagnostics(remove);
                    }
                }

                notifier(diagnosticsUpdatedArgs);
            }
        }

    }
}

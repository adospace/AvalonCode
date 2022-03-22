using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvalonCode.Tests;

[TestClass]
public class UnitTest1
{
    [AssemblyInitialize()]
    public static void AssemblyInit(TestContext context)
    {
        MSBuildLocator.RegisterDefaults();
    }
    
    [TestMethod]
    public async Task TestBasicCompletion()
    {
        var assemblies = new[]
        {
            typeof(string).Assembly,
            typeof(IList<string>).Assembly,
            Assembly.Load("Microsoft.CodeAnalysis"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp"),
            Assembly.Load("Microsoft.CodeAnalysis.Features"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
        };

        var partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies)
            .Distinct()
            .SelectMany(x => x.GetTypes())
            .ToArray();

        var compositionContext = new ContainerConfiguration()
            .WithParts(partTypes)
            .CreateContainer();

        MefHostServices host = MefHostServices.Create(compositionContext);

        var workspace = new AdhocWorkspace(host);

        string code = @"using System;
namespace Test {
    public class TestClass {
        public IList<string> TestMethod() {
            string x = ""test"";
            x.
        }
    }
}";
        var document = workspace
            .AddProject("TestProject", LanguageNames.CSharp)
            .AddDocument("TestDocument.cs", SourceText.From(code));

        var service = CompletionService.GetService(document);
        int idx = code.IndexOf("x.", StringComparison.Ordinal) + 1;
        var list = await service.GetCompletionsAsync(document, idx, CompletionTrigger.CreateInsertionTrigger('.'));

        Assert.IsNotNull(list);
        Assert.IsTrue(list.Items.Any());

        var signatureHelperProvider = QuickInfoService.GetService(document) ?? throw new InvalidOperationException();

        var quickInfo = await signatureHelperProvider.GetQuickInfoAsync(document, idx - 1);
        Assert.IsNotNull(quickInfo);
        Assert.IsTrue(quickInfo.Sections.Any());
    }

    [TestMethod]
    public async Task TestBasicCompletionWithGlobalUsings()
    {
        var assemblies = new[]
        {
            typeof(string).Assembly,
            typeof(IList<string>).Assembly,
            Assembly.Load("Microsoft.CodeAnalysis"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp"),
            Assembly.Load("Microsoft.CodeAnalysis.Features"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
        };

        var partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies)
            .Distinct()
            .SelectMany(x => x.GetTypes())
            .ToArray();

        var compositionContext = new ContainerConfiguration()
            .WithParts(partTypes)
            .CreateContainer();

        MefHostServices host = MefHostServices.Create(compositionContext);

        var workspace = new AdhocWorkspace(host);

        string code = @"Console.WriteLine(";
        var document = workspace
            .AddProject("TestProject", LanguageNames.CSharp)
            .AddDocument("TestDocument.cs", SourceText.From(code));

        var service = CompletionService.GetService(document);
        int idx = code.IndexOf("(", StringComparison.Ordinal) + 1;
        var list = await service.GetCompletionsAsync(document, idx, CompletionTrigger.CreateInsertionTrigger('('));

        Assert.IsNotNull(list);
        Assert.IsTrue(list.Items.Any());

        var signatureHelperProvider = QuickInfoService.GetService(document) ?? throw new InvalidOperationException();

        QuickInfoItem? quickInfo = await signatureHelperProvider.GetQuickInfoAsync(document, idx);
        Assert.IsNotNull(quickInfo);
        Assert.IsTrue(quickInfo.Sections.Any());
    }

    [TestMethod]
    public async Task TestBasicCompletionUsingMsBuildWorkspace()
    {
        var assemblies = new[]
        {
            typeof(string).Assembly,
            typeof(IList<string>).Assembly,
            Assembly.Load("Microsoft.CodeAnalysis"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp"),
            Assembly.Load("Microsoft.CodeAnalysis.Features"),
            Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"),
        };

        var partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies)
            .Distinct()
            .SelectMany(x => x.GetTypes())
            .ToArray();

        var compositionContext = new ContainerConfiguration()
            .WithParts(partTypes)
            .CreateContainer();

        MefHostServices host = MefHostServices.Create(compositionContext);

        var workspace = MSBuildWorkspace.Create();

        var project = await workspace.OpenProjectAsync(@"..\..\..\..\AvalonCode.Tests.ConsoleProgram\AvalonCode.Tests.ConsoleProgram.csproj");

        var document = project.Documents.First(_ => _.Name == "Program.cs");

        var completionService = CompletionService.GetService(document);
        var list = await completionService.GetCompletionsAsync(document, 18, CompletionTrigger.CreateInsertionTrigger('('));

        Assert.IsNotNull(list);
        Assert.IsTrue(list.Items.Any());

        var signatureHelperProvider = QuickInfoService.GetService(document) ?? throw new InvalidOperationException();

        var quickInfo = await signatureHelperProvider.GetQuickInfoAsync(document, 5);
        Assert.IsNotNull(quickInfo);
        Assert.IsTrue(quickInfo.Sections.Any());
    }
    
}
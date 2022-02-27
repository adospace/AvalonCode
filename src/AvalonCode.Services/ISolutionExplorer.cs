using AvalonCode.Services.Models;
using System.Reflection;

namespace AvalonCode.Services
{
    public interface ISolutionExplorer
    {
        SolutionHost? CurrentSolution { get; }

        Task<SolutionHost> OpenSolution(string solutionFilePath, IEnumerable<Assembly>? additionalAssemblies = null, CancellationToken cancellationToken = default);

        Task CloseSolution();
    }
}
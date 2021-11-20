using AvalonCode.Services.Models;

namespace AvalonCode.Services
{
    public interface ISolutionExplorer
    {
        SolutionInfo? CurrentSolution { get; }

        Task<SolutionInfo> OpenSolution(string solutionFilePath, CancellationToken cancellationToken = default);

        Task CloseSolution();
    }
}
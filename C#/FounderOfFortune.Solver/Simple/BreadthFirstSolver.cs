using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;

namespace FounderOfFortune.Solver.Simple;

public class BreadthFirstSolver(ILogger<Solver> logger) : Solver(logger)
{
    private readonly Queue<Solution> _solutions = new();

    protected override Solution GetNextSolution() => _solutions.Dequeue();
    protected override bool CanGetNextSolution() => _solutions.Count != 0;
    protected override void StoreSolution(Solution solution) => _solutions.Enqueue(solution);
}
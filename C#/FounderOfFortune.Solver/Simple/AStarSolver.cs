using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;

namespace FounderOfFortune.Solver.Simple;

public class AStarSolver(ILogger<Solver> logger, Func<Solution, int> heuristic) : Solver(logger)
{
    private readonly PriorityQueue<Solution, int> _queue = new ();

    protected override Solution GetNextSolution() => _queue.Dequeue();
    protected override bool CanGetNextSolution()
    {
        Logger.LogTrace("Items in queue: {Count}", _queue.Count);
        return _queue.Count > 0;
    }
    protected override void StoreSolution(Solution solution)
    {
        var priority = heuristic(solution);
        Logger.LogTrace("Board evaluated by heuristic to {Priority}", priority);
        _queue.Enqueue(solution, priority);
    }
}
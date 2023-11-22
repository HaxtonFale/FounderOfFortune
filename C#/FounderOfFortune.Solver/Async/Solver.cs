using System.Threading.Tasks.Dataflow;
using FounderOfFortune.Game;
using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;

namespace FounderOfFortune.Solver.Async;

public class Solver(ILogger<Solver> logger)
{
    protected readonly ILogger<Solver> Logger = logger;

    public async Task<Solution?> TrySolve(BoardState initialState, bool moveSingleCards, CancellationToken token = default)
    {
        var initialSolution = new Solution(initialState);
        var queue = new BufferBlock<Solution>(new DataflowBlockOptions
        {
            CancellationToken = token
        });
        queue.Post(initialSolution);

        var visitedStates = new HashSet<BoardState> { initialState };
        var generator = new TransformManyBlock<Solution, Solution>(solution =>
        {
            Logger.LogDebug("Generating new solutions from a single solution...");
            return solution.Board.GetValidActions()
                .Select(action => (solution.Board.PerformAction(action, moveSingleCards), action))
                .Where(tuple => visitedStates.Add(tuple.Item1))
                .Select(tuple => solution.AddStep(tuple.Item1, tuple.action));
        }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = token
            });

        var solved = new BufferBlock<Solution>(new DataflowBlockOptions
        {
            BoundedCapacity = 1,
            CancellationToken = token
        });

        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };
        try
        {
            using var link1 = generator.LinkTo(solved, linkOptions, solution => solution.IsDone);
            using var link2 = generator.LinkTo(queue, linkOptions, solution => !solution.IsDone);
            using var link3 = queue.LinkTo(generator, linkOptions);

            var result = await solved.ReceiveAsync(token);
            solved.Complete();
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred.");
            return null;
        }
    }

}
using System.Collections.Concurrent;
using ConcurrentPriorityQueue;
using FounderOfFortune.Game;
using FounderOfFortune.Solver.Model;

namespace FounderOfFortune.Solver.Parallel;

public class TaskSolver
{
    private readonly ConcurrentPriorityByIntegerQueue<Solution> _queue = new();
    private const int TaskCount = 20;

    public async Task<Solution?> TrySolve(BoardState initialState, bool moveSingleCards, CancellationToken token)
    {
        _queue.Enqueue(new Solution(initialState));

        var visitedStates = new ConcurrentDictionary<BoardState, bool>();
        visitedStates.TryAdd(initialState, true);

        var tasks = Enumerable.Repeat(Task.Run(TrySolveSingle, token), TaskCount).ToArray();

        var results = await Task.WhenAll(tasks);

        return results.FirstOrDefault(s => s != null);

        async Task<Solution?> TrySolveSingle()
        {
            while (!token.IsCancellationRequested)
            {
                if (!_queue.TryTake(out var solution))
                {
                    await Task.Delay(100, token);
                    continue;
                }
                foreach (var action in solution.Board.GetValidActions())
                {
                    var boardState = solution.Board.PerformAction(action, moveSingleCards);
                    if (!visitedStates.TryAdd(boardState, true)) continue;
                    var newSolution = solution.AddStep(boardState, action);
                    if (newSolution.IsDone) return newSolution;
                    _queue.Enqueue(newSolution);
                }
            }

            return null;
        }
    }
}
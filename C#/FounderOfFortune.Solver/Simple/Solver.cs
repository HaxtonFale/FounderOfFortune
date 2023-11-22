using FounderOfFortune.Game;
using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;

namespace FounderOfFortune.Solver.Simple;

public abstract class Solver(ILogger<Solver> logger)
{
    protected readonly ILogger<Solver> Logger = logger;

    protected abstract Solution GetNextSolution();
    protected abstract bool CanGetNextSolution();
    protected abstract void StoreSolution(Solution solution);

    public Solution? TrySolve(BoardState initialState, bool moveSingleCards)
    {
        StoreSolution(new Solution(initialState));

        var visitedStates = new HashSet<BoardState> {initialState};
        var solutions = 0;

        while (CanGetNextSolution())
        {
            Logger.LogInformation("Testing a solution...");
            var solution = GetNextSolution();
            Logger.LogDebug("Solutions seen: {Count}. Current solution length: {Steps} steps.", solutions++, solution.Length);
            foreach (var action in solution.Board.GetValidActions())
            {
                var state = solution.Board.PerformAction(action, moveSingleCards);
                if (!visitedStates.Add(state)) continue;
                var newSolution = solution.AddStep(state, action);
                if (newSolution.IsDone) return newSolution;
                StoreSolution(newSolution);
            }
        }

        return null;
    }
}
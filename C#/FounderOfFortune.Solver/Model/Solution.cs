using ConcurrentPriorityQueue.Core;
using FounderOfFortune.Game;

namespace FounderOfFortune.Solver.Model;

public class Solution : IHavePriority<int>
{
    public BoardState Board { get; init; }
    public (Solution Solution, GameAction Action)? PreviousStep { get; init; }
    public uint Length { get; }
    public bool IsDone { get; }

    public Solution(BoardState board)
    {
        Board = board;
        Length = 0;
        IsDone = board.IsComplete();
    }

    private Solution(BoardState board, (Solution Solution, GameAction action) previousStep)
    {
        Board = board;
        PreviousStep = previousStep;
        Length = 1 + previousStep.Solution.Length;
        IsDone = board.IsComplete();
    }

    public Solution AddStep(BoardState board, GameAction action)
    {
        return new Solution(board, (this, action));
    }

    public IEnumerable<string> RenderSolution()
    {
        var current = this;
        while (current.PreviousStep != null)
        {
            var (prevStep, action) = current.PreviousStep.Value;
            yield return action switch
            {
                MoveCard m => $"Move a card from stack {m.From + 1} to stack {m.To + 1}.",
                StoreCard s => $"Place a card from stack {s.From + 1} on the free cell.",
                RetrieveCard r => $"Retrieve a card from the free cell to stack {r.To + 1}.",
                _ => throw new ArgumentOutOfRangeException()
            };
            current = prevStep;
        }
    }

    public int Priority => Heuristics.DepthAndRuns(this);
}
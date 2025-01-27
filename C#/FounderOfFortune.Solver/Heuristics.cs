using FounderOfFortune.Game;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Solver;

public static class Heuristics
{
    public static int RunCount(BoardState board)
    {
        var runs = 0;
        foreach (var stack in board.TableauStacks.Select(stack => stack.Cards).Where(cards => cards.Count != 0))
        {
            runs++;
            if (stack.Count == 1) continue;

            for (var i = 1; i < stack.Count; i++)
            {
                if (!stack[i - 1].IsAdjacentTo(stack[i])) runs++;
            }
        }

        return runs;
    }

    public static int TerminalDepth(BoardState board)
    {
        var totalDepth = 0;
        var terminals = new HashSet<Card>();
        if (board.MajorArcanaStacks.Left == null) terminals.Add(new MajorArcana(0));
        if (board.MajorArcanaStacks.Right == null) terminals.Add(new MajorArcana(21));
        if (board.MajorArcanaStacks is { Left: not null, Right: not null } &&
            board.MajorArcanaStacks.Left != board.MajorArcanaStacks.Right)
        {
            terminals.Add(board.MajorArcanaStacks.Left + 1);
            terminals.Add(board.MajorArcanaStacks.Right - 1);
        }

        foreach (var suit in Enum.GetValues<Suit>())
        {
            var topCard = board.MinorArcanaStacks.TopCard(suit);
            if (topCard.Value < 13) terminals.Add(topCard + 1);
        }

        foreach (var stack in board.TableauStacks.Select(s => s.Cards))
        {
            for (var i = 0; i < stack.Count; i++)
            {
                if (terminals.Contains(stack[i])) totalDepth += stack.Count - i;
            }
        }

        return totalDepth;
    }

    public static int DepthAndRuns(BoardState board) => RunCount(board) * 1000 + TerminalDepth(board);
}
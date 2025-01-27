using System.Text;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Model;

namespace FounderOfFortune.Solver;

public static class BoardStateExtensions
{
    public static BoardState PerformAction(this BoardState state, GameAction action, bool moveSingleCards = false) =>
        action switch
        {
            MoveCard m => moveSingleCards ? state.MoveSingleCard(m.From, m.To) : state.MoveCardSequence(m.From, m.To),
            StoreCard s => state.StoreCard(s.From),
            RetrieveCard r => state.RetrieveCard(r.To),
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

    public static bool IsComplete(this BoardState board) =>
        board.TableauStacks.All(s => s.Cards.IsEmpty)
        && board.MajorArcanaStacks.Left == board.MajorArcanaStacks.Right
        && board.MinorArcanaStacks.Stacks.All(s => s.TopCard.Value == 13);

    public static string RenderBoard(this BoardState board)
    {
        var maxStackHeight = board.TableauStacks.Select(s => s.Cards.Count).Max();
        if (maxStackHeight == 0) return "Done.";
        var output = new StringBuilder();
        var leftCard = board.MajorArcanaStacks.Left switch
        {
            null => "",
            { Value: 0 } => "0",
            { Value: 1 } => ".I",
            { } m => $"..{m}"
        };
        var rightCard = board.MajorArcanaStacks.Right switch
        {
            null => "",
            { Value: 21 } => "XXI",
            { Value: 1 } => "XX.",
            { } m => $"{m}.."
        };
        var middle = string.Join("", Enumerable.Repeat(' ', 16 - leftCard.Length - rightCard.Length));
        output.Append(leftCard);
        output.Append(middle);
        output.Append(rightCard);
        var minorArcana = board.MinorArcanaStacks.Stacks.Select(s => PrintCard(s.TopCard).PadRight(5, ' '))
            .Aggregate((s1, s2) => s1 + s2).PadLeft(34, ' ');
        output.AppendLine(minorArcana);
        output.AppendLine(string.Join("", Enumerable.Repeat('-', 51)));
        for (var height = 0; height < maxStackHeight; height++)
        {
            for (var stackIndex = 0; stackIndex < 11; stackIndex++)
            {
                var stack = board.TableauStacks[stackIndex];
                output.Append(stack.Cards.Count <= height ? "     " : PrintCard(stack.Cards[height]).PadRight(5, ' '));
            }

            output.AppendLine();
        }
        return output.ToString().TrimEnd('\n');
    }

    private static string PrintCard(Card card)
    {
        if (card is MajorArcana major)
        {
            return major.ToString();
        }
        else
        {
            var mi = (MinorArcana) card;
            var suit = mi.Suit switch
            {
                Suit.Coins => "C",
                Suit.Goblets => "G",
                Suit.Swords => "S",
                Suit.Wands => "W",
                _ => throw new ArgumentOutOfRangeException()
            };
            return suit + mi.Value;
        }
    }
    public static IEnumerable<GameAction> GetValidActions(this BoardState state)
    {
        foreach (var (from, to) in StackPairs)
        {
            if (state.TableauStacks[from].Cards.IsEmpty) continue;
            if (state.TableauStacks[to].Cards.IsEmpty
                || state.TableauStacks[to].TopCard!.IsAdjacentTo(state.TableauStacks[from].TopCard!))
                yield return new MoveCard(from, to);
        }

        if (state.FreeCell == null)
        {
            foreach (var stack in Stacks)
            {
                if (state.TableauStacks[stack].IsEmpty) continue;
                yield return new StoreCard(stack);
            }
        }
        else
        {
            var fcCard = state.FreeCell;
            foreach (var stack in Stacks)
            {
                var stackCard = state.TableauStacks[stack].TopCard;
                if (stackCard == null || stackCard.IsAdjacentTo(fcCard)) yield return new RetrieveCard(stack);
            }
        }
    }

    private static IEnumerable<int> Stacks => Enumerable.Range(0, 11);

    private static IEnumerable<(int from, int to)> StackPairs
    {
        get
        {
            for (var i = 0; i < 11; i++)
            {
                for (var j = 0; j < 11; j++)
                {
                    if (i == j) continue;
                    yield return (i, j);
                }
            }
        }
    }
}
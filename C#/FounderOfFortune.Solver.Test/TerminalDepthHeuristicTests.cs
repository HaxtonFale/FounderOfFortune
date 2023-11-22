using System.Collections.Immutable;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Solver.Test;

public class TerminalDepthHeuristicTests : HeuristicTestsBase
{
    [Fact]
    public void EmptyBoardReturnsZero()
    {
        // Arrange
        var solution = new Solution(EmptyBoard);

        // Act
        var value = Heuristics.TerminalDepth(solution);

        // Assert
        value.Should().Be(0);
    }

    [Fact]
    public void CorrectlyReturnsTerminalDepth()
    {
        // Arrange
        var cards = new List<Card> { new MajorArcana(0), new MinorArcana(Suit.Coins, 5) };
        var stacks = EmptyStacks.SetItem(0, new TableauStack(cards.ToImmutableList()));
        var solution = new Solution(new BoardState(stacks));

        // Act
        var value = Heuristics.TerminalDepth(solution);

        // Assert
        value.Should().Be(2);
    }
}
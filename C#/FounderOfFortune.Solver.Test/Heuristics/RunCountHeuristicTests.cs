using System.Collections.Immutable;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Solver.Test.Heuristics;

public class RunCountHeuristicTests : HeuristicTestsBase
{
    [Fact]
    public void EmptyBoardReturnsZero()
    {
        // Act
        var value = Solver.Heuristics.RunCount(EmptyBoard);

        // Assert
        value.Should().Be(0);
    }

    [Fact]
    public void SingleCardReturnsOne()
    {
        // Arrange
        var stacks = EmptyStacks.SetItem(0, EmptyStacks[0].PlaceCard(new MajorArcana(3)));
        var board = new BoardState(stacks);

        // Act
        var value = Solver.Heuristics.RunCount(board);

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public void JustConsecutiveCardsReturnOne()
    {
        // Arrange
        var sequence = new CardSequence(new MajorArcana(3), 10);
        var stacks = EmptyStacks.SetItem(0, EmptyStacks[0].PlaceRange(sequence));
        var solution = new BoardState(stacks);

        // Act
        var value = Solver.Heuristics.RunCount(solution);

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public void MultipleCardRunsReturnRunCount()
    {
        // Arrange
        var sequence =
            new CardSequence(new MajorArcana(3), 10).Concat(new CardSequence(new MinorArcana(Suit.Coins, 10), 3));
        var stacks = EmptyStacks.SetItem(0, new TableauStack(sequence.ToImmutableList()));
        var board = new BoardState(stacks);

        // Act
        var value = Solver.Heuristics.RunCount(board);

        // Assert
        value.Should().Be(2);
    }

    [Fact]
    public void OneCardOnEachStackReturnsStackCount()
    {
        // Arrange
        var stacks = EmptyStacks.Select(s => s.PlaceCard(new MajorArcana(3))).ToImmutableList();
        var board = new BoardState(stacks);

        // Act
        var value = Solver.Heuristics.RunCount(board);

        // Assert
        value.Should().Be(stacks.Count);
    }


}
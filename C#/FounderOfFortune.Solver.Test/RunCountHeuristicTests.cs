﻿using System.Collections.Immutable;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Solver.Test;

public class RunCountHeuristicTests : HeuristicTestsBase
{
    [Fact]
    public void EmptyBoardReturnsZero()
    {
        // Arrange
        var solution = new Solution(EmptyBoard);

        // Act
        var value = Heuristics.RunCount(solution);

        // Assert
        value.Should().Be(0);
    }

    [Fact]
    public void SingleCardReturnsOne()
    {
        // Arrange
        var stacks = EmptyStacks.SetItem(0, EmptyStacks[0].PlaceCard(new MajorArcana(3)));
        var solution = new Solution(new BoardState(stacks));

        // Act
        var value = Heuristics.RunCount(solution);

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public void JustConsecutiveCardsReturnOne()
    {
        // Arrange
        var sequence = new CardSequence(new MajorArcana(3), 10);
        var stacks = EmptyStacks.SetItem(0, EmptyStacks[0].PlaceRange(sequence));
        var solution = new Solution(new BoardState(stacks));

        // Act
        var value = Heuristics.RunCount(solution);

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
        var solution = new Solution(new BoardState(stacks));

        // Act
        var value = Heuristics.RunCount(solution);

        // Assert
        value.Should().Be(2);
    }

    [Fact]
    public void OneCardOnEachStackReturnsStackCount()
    {
        // Arrange
        var stacks = EmptyStacks.Select(s => s.PlaceCard(new MajorArcana(3))).ToImmutableList();
        var solution = new Solution(new BoardState(stacks));

        // Act
        var value = Heuristics.RunCount(solution);

        // Assert
        value.Should().Be(stacks.Count);
    }


}
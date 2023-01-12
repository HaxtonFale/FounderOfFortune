using System.Collections.Immutable;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class TableauStackTest {
    [Fact]
    public void DefaultConstructorCreatesEmptyStack() {
        // Arrange
        var stack = new TableauStack();

        // Assert
        Assert.Empty(stack.Cards);
    }

    [Fact]
    public void TakeFullStackIfCardsMakeASequence() {
        // Arrange
        var cards = Enumerable.Range(1, 10).Select(n => new Card(new MajorArcana(n))).ToImmutableList();
        var stack = new TableauStack(cards);

        // Act
        var newCards = stack.TakeCards(out var newStack);

        // Assert
        Assert.Equal(cards.Reverse(), newCards);
        Assert.Empty(newStack.Cards);
    }
}
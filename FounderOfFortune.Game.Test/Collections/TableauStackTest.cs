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

    [Fact]
    public void PlaceAnyCardOnEmptyStack()
    {
        // Arrange
        var card = new MinorArcana(Suit.Coins, 3);
        var stack = new TableauStack();

        // Act
        var newStack = stack.PlaceCard(card);

        // Assert
        Assert.Equal(new List<Card> {card}, newStack.Cards);
    }

    [Fact]
    public void PlaceValidCardOnNonEmptyStack() {
        // Arrange
        var threeOfCoins = new MinorArcana(Suit.Coins, 3);
        var fourOfCoins = new MinorArcana(Suit.Coins, 4);
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(fourOfCoins));

        // Act
        var newStack = stack.PlaceCard(threeOfCoins);

        // Assert
        Assert.Equal(new List<Card> { fourOfCoins, threeOfCoins }, newStack.Cards);
    }

    [Fact]
    public void PlaceInvalidCardOnNonEmptyStack() {
        // Arrange
        var threeOfCoins = new MinorArcana(Suit.Coins, 3);
        var fourOfSwords = new MinorArcana(Suit.Swords, 4);
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(fourOfSwords));

        // Assert
        Assert.Throws<ArgumentException>(() => stack.PlaceCard(threeOfCoins));
    }
}
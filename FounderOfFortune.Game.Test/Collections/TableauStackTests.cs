using System.Collections.Immutable;
using FluentAssertions;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class TableauStackTests {
    [Fact]
    public void DefaultConstructorCreatesEmptyStack() {
        // Arrange
        var stack = new TableauStack();

        // Assert
        stack.Cards.Should().BeEmpty();
    }

    [Fact]
    public void TakeFullStackIfCardsMakeASequence() {
        // Arrange
        var cards = Enumerable.Range(1, 10).Select(n => new Card(new MajorArcana(n))).ToImmutableList();
        var stack = new TableauStack(cards);

        // Act
        var newCards = stack.TakeCards(out var newStack);

        // Assert
        newCards.Should().BeEquivalentTo(cards.Reverse());
        newStack.Cards.Should().BeEmpty();
    }

    [Fact]
    public void TakeCorrectSequenceFromMixedStack() {
        // Arrange
        var cards = Enumerable.Range(3, 5).Select(n => new Card(new MinorArcana(Suit.Coins, n))).ToImmutableList();
        var topCards = Enumerable.Range(5, 3).Select(n => new Card(new MinorArcana(Suit.Swords, n))).ToImmutableList();
        var stack = new TableauStack(cards.AddRange(topCards));

        // Act
        var takenCards = stack.TakeCards(out var newStack);

        // Assert
        takenCards.Should().BeEquivalentTo(topCards.Reverse());
        newStack.Cards.Should().BeEquivalentTo(cards);
    }

    [Fact]
    public void TakingCardFromEmptyStackThrows()
    {
        // Arrange
        var stack = new TableauStack();

        // Act
        var action = () => stack.TakeCard(out _);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void TakingCardsFromEmptyStackThrows() {
        // Arrange
        var stack = new TableauStack();

        // Act
        var action = () => stack.TakeCards(out _);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void PlaceAnyCardOnEmptyStack() {
        // Arrange
        var card = new MinorArcana(Suit.Coins, 3);
        var stack = new TableauStack();

        // Act
        var newStack = stack.PlaceCard(card);

        // Assert
        newStack.Cards.Should().BeEquivalentTo(new List<Card> { card });
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
        newStack.Cards.Should().BeEquivalentTo(new List<Card> { fourOfCoins, threeOfCoins });
    }

    [Fact]
    public void PlaceInvalidCardOnNonEmptyStackThrows() {
        // Arrange
        var threeOfCoins = new MinorArcana(Suit.Coins, 3);
        var fourOfSwords = new MinorArcana(Suit.Swords, 4);
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(fourOfSwords));

        // Act
        var action = () => stack.PlaceCard(threeOfCoins);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void PlaceAnyCardRangeOnEmptyStack() {
        // Arrange
        var cards = Enumerable.Range(4, 7).Select(n => new Card(new MinorArcana(Suit.Coins, n))).ToList();
        var stack = new TableauStack();

        // Act
        var newStack = stack.PlaceRange(cards);

        // Assert
        newStack.Cards.Should().BeEquivalentTo(cards);
    }

    [Fact]
    public void PlaceValidCardRangeOnNonEmptyStack() {
        // Arrange
        var threeOfCoins = ImmutableList<Card>.Empty.Add(new MinorArcana(Suit.Coins, 3));
        var cards = Enumerable.Range(4, 7).Select(n => new Card(new MinorArcana(Suit.Coins, n))).ToImmutableList();
        var stack = new TableauStack(threeOfCoins);

        // Act
        var newStack = stack.PlaceRange(cards);

        // Assert
        newStack.Cards.Should().BeEquivalentTo(threeOfCoins.AddRange(cards));
    }

    [Fact]
    public void PlaceInvalidCardRangeOnNonEmptyStackThrows() {
        // Arrange
        var threeOfCoins = ImmutableList<Card>.Empty.Add(new MinorArcana(Suit.Goblets, 3));
        var cards = Enumerable.Range(4, 7).Select(n => new Card(new MinorArcana(Suit.Coins, n)));
        var stack = new TableauStack(threeOfCoins);

        // Act
        var action = () => stack.PlaceRange(cards);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }
}
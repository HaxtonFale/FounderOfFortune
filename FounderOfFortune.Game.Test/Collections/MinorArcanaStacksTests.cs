using FluentAssertions;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections; 

public class MinorArcanaStacksTests {
    [Fact]
    public void DefaultConstructorCreatesStacksWithAces() {
        // Arrange
        var expected = Enum.GetValues<Suit>().ToDictionary(s => s, s => new MinorArcana(s, 1));

        // Act
        var stacks = new MinorArcanaStacks();

        // Assert
        stacks.TopCards.Should().BeEquivalentTo(expected);
        foreach (var suit in expected.Keys) {
            stacks.TopCard(suit).Should().Be(expected[suit]);
        }
    }

    [Fact]
    public void ValidAscensionAllowed() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var card = new MinorArcana(Suit.Goblets, 2);

        // Act
        var result = stacks.CanAscend(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidAscensionSucceeds() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var card = new MinorArcana(Suit.Goblets, 2);

        // Act
        stacks = stacks.Ascend(card);

        // Assert
        stacks.TopCard(Suit.Goblets).Should().Be(card);
        foreach (var suit in Enum.GetValues<Suit>().Where(s => s != card.Suit)) {
            stacks.TopCard(suit).Should().Be(new MinorArcana(suit, 1));
        }
    }

    [Fact]
    public void InvalidAscensionNotAllowed() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var card = new MinorArcana(Suit.Goblets, 3);

        // Act
        var result = stacks.CanAscend(card);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InvalidAscensionThrows() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var card = new MinorArcana(Suit.Goblets, 3);

        // Act
        var action = () => stacks.Ascend(card);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Card value ineligible for ascension");
    }

    [Fact]
    public void ValidAscensionOfCardSequenceSucceeds() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var range = new CardSequence(new MinorArcana(Suit.Swords, 2), 4);

        // Act
        stacks = stacks.AscendRange(range.Select(c => c.AsMinorArcana));

        // Assert
        stacks.TopCard(Suit.Swords).Should().Be(new MinorArcana(Suit.Swords, 4));
        foreach (var suit in Enum.GetValues<Suit>().Where(s => s != Suit.Swords)) {
            stacks.TopCard(suit).Should().Be(new MinorArcana(suit, 1));
        }
    }

    [Fact]
    public void ValidAscensionOfMixedCardsSucceeds() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var range = Enum.GetValues<Suit>().Select(s => new MinorArcana(s, 2)).ToList();

        // Act
        stacks = stacks.AscendRange(range);

        // Assert
        stacks.TopCards.Should().BeEquivalentTo(range.ToDictionary(c => c.Suit));
    }

    [Fact]
    public void InvalidAscensionOfCardSequenceThrows() {
        // Arrange
        var stacks = new MinorArcanaStacks();
        var range = new CardSequence(new MinorArcana(Suit.Swords, 4), 2);

        // Act
        var action = () => stacks.AscendRange(range.Select(c => c.AsMinorArcana));

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Card value ineligible for ascension");
    }
}
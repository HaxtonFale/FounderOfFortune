using FluentAssertions;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class MinorArcanaStackTests {
    [Fact]
    public void ValidAscensionUpdatesTopCard() {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Coins, 4);

        // Act
        stack = stack.Ascend(card);

        // Assert
        stack.TopCard.Should().Be(card);
    }

    [Fact]
    public void ValidAscensionAllowed() {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Coins, 4);

        // Act
        var result = stack.CanAscend(card);

        // Assert
        result.Should().BeTrue();
    }
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void ValueMismatchInvalidAscensionNotAllowed(int value) {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Coins, value);

        // Act
        var result = stack.CanAscend(card);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void ValueMismatchInvalidAscensionThrows(int value) {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Coins, value);

        // Act
        var action = () => stack.Ascend(card);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Card value ineligible for ascension");
    }

    [Fact]
    public void SuitMismatchInvalidAscensionNotAllowed() {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Goblets, 4);

        // Act
        var result = stack.CanAscend(card);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SuitMismatchInvalidAscensionThrows() {
        // Arrange
        var stack = new MinorArcanaStack(new MinorArcana(Suit.Coins, 3));
        var card = new MinorArcana(Suit.Goblets, 4);

        // Act
        var action = () => stack.Ascend(card);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Card suit mismatch");
    }
}
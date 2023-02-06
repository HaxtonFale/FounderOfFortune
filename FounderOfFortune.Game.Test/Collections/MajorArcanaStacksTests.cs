using FluentAssertions;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections; 

public class MajorArcanaStacksTests {
    [Fact]
    public void DefaultConstructorCreatesEmptyStacks() {
        // Arrange & Act
        var stacks = new MajorArcanaStacks();

        // Assert
        stacks.LeftCard.Should().BeNull("left stack is empty");
        stacks.RightCard.Should().BeNull("right stack is empty");
    }

    [Fact]
    public void ValidAscensionUpdatesLeftCard() {
        // Arrange
        var stacks = new MajorArcanaStacks();
        var card = new MajorArcana(0);

        // Act
        stacks = stacks.Ascend(card);

        // Assert
        stacks.LeftCard.Should().Be(card, "the card was ascended to the left stack");
        stacks.RightCard.Should().BeNull("no card was ascended to the right stack");
    }

    [Fact]
    public void ValidAscensionUpdatesRightCard() {
        // Arrange
        var stacks = new MajorArcanaStacks();
        var card = new MajorArcana(21);

        // Act
        stacks = stacks.Ascend(card);

        // Assert
        stacks.LeftCard.Should().BeNull("no card was ascended to the left stack");
        stacks.RightCard.Should().Be(card, "the card was ascended to the right stack");
    }

    [Fact]
    public void ValidAscensionAllowed() {
        // Arrange
        var stacks = new MajorArcanaStacks();
        var left = new MajorArcana(0);
        var right = new MajorArcana(21);

        // Act
        var leftResult = stacks.CanAscend(left);
        var rightResult = stacks.CanAscend(right);

        // Assert
        leftResult.Should().BeTrue("left card is 0 and left stack is empty");
        rightResult.Should().BeTrue("right card is 21 and right stack is empty");
    }

    [Theory]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(14)]
    public void InvalidAscensionThrows(int value) {
        // Arrange
        var stacks = new MajorArcanaStacks(0, 13);
        var card = new MajorArcana(value);

        // Act
        var action = () => stacks.Ascend(card);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Card value ineligible for ascension");
    }

    [Theory]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(14)]
    public void InvalidAscensionNotAllowed(int value) {
        // Arrange
        var stacks = new MajorArcanaStacks(0, 13);
        var card = new MajorArcana(value);

        // Act
        var result = stacks.CanAscend(card);

        // Assert
        result.Should().BeFalse("card value is neither 1 (for left stack ascension) nor 12 (for right stack ascension)");
    }

    [Fact]
    public void CannotAscendToFullStack() {
        // Arrange
        var stacks = new MajorArcanaStacks(7, 7);
        var card = new MajorArcana(6);

        // Act
        var result = stacks.CanAscend(card);

        // Assert
        result.Should().BeFalse("stack is full and cannot accept more cards");
    }

    [Fact]
    public void CannotCreateStacksCrossingEachOther() {
        // Arrange & Act
        var action = () => new MajorArcanaStacks(8, 6);

        // Assert
        action.Should().Throw<ArgumentException>("left stack cannot exceed right stack")
            .WithMessage("Left cannot be greater than right.");
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(22)]
    [InlineData(23)]
    public void CannotCreateStacksWithInvalidLeftValue(int leftValue) {
        // Arrange & Act
        var action = () => new MajorArcanaStacks(leftValue, 22);

        // Assert
        action.Should().Throw<ArgumentException>("left stack must be between -1 and 21");
    }

    [Theory]
    [InlineData(23)]
    [InlineData(-1)]
    [InlineData(-2)]
    public void CannotCreateStacksWithInvalidRightValue(int rightValue) {
        // Arrange & Act
        var action = () => new MajorArcanaStacks(-1, rightValue);

        // Assert
        action.Should().Throw<ArgumentException>("right stack must be between 0 and 22");
    }
}
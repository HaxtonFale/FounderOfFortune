using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using PromotionTestData = Xunit.TheoryData<FounderOfFortune.Game.Model.MinorArcana, FounderOfFortune.Game.Model.MinorArcana>;

namespace FounderOfFortune.Game.Test.Collections;

public class MinorArcanaStackTests {
    public static PromotionTestData ValidPromotions {
        get {
            var data = new PromotionTestData();
            foreach (var suit in Enum.GetValues<Suit>()) {
                for (var i = 1; i < MinorArcana.MaxValue; i++) {
                    var card = new MinorArcana(suit, i);
                    data.Add(card, card + 1);
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ValidPromotions))]
    public void ValidCardsCanPromote(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Assert
        Assert.True(stack.CanPromote(nextCard));
    }

    [Theory]
    [MemberData(nameof(ValidPromotions))]
    public void ValidPromotionsAdvanceStack(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Act
        var newStack = stack.Promote(nextCard);

        // Assert
        Assert.Equal(nextCard, newStack.TopCard);
    }

    public static PromotionTestData SuitMismatchPromotions {
        get {
            var data = new PromotionTestData();
            foreach (var suit in Enum.GetValues<Suit>()) {
                foreach (var otherSuit in Enum.GetValues<Suit>()) {
                    if (otherSuit == suit) continue;
                    for (var i = 1; i < MinorArcana.MaxValue; i++) {
                        data.Add(new MinorArcana(suit, i), new MinorArcana(otherSuit, i + 1));
                    }
                }
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(SuitMismatchPromotions))]
    public void CannotPromoteWithMismatchedSuits(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Assert
        Assert.False(stack.CanPromote(nextCard));
    }

    [Theory]
    [MemberData(nameof(SuitMismatchPromotions))]
    public void SuitMismatchPromotionCausesException(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => stack.Promote(nextCard));
        Assert.StartsWith("Card suit mismatch", exception.Message);
    }

    public static PromotionTestData ValueMismatchPromotions {
        get {
            var data = new PromotionTestData();
            foreach (var suit in Enum.GetValues<Suit>()) {
                for (var i = 1; i < MinorArcana.MaxValue; i++) {
                    for (var j = 1; j <= MinorArcana.MaxValue; j++) {
                        if (i + 1 == j) continue;
                        data.Add(new MinorArcana(suit, i), new MinorArcana(suit, j));
                    }
                }
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ValueMismatchPromotions))]
    public void CannotPromoteWithInvalidValue(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Assert
        Assert.False(stack.CanPromote(nextCard));
    }

    [Theory]
    [MemberData(nameof(ValueMismatchPromotions))]
    public void InvalidValuePromotionCausesException(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => stack.Promote(nextCard));
        Assert.StartsWith("Card value ineligible for promotion", exception.Message);
    }

    public static PromotionTestData SuitAndValueMismatchPromotions {
        get {
            var data = new PromotionTestData();
            foreach (var suit in Enum.GetValues<Suit>()) {
                foreach (var otherSuit in Enum.GetValues<Suit>()) {
                    if (otherSuit == suit) continue;
                    for (var i = 1; i < MinorArcana.MaxValue; i++) {
                        for (var j = 1; j <= MinorArcana.MaxValue; j++) {
                            if (i + 1 == j) continue;
                            data.Add(new MinorArcana(suit, i), new MinorArcana(otherSuit, j));
                        }
                    }
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(SuitAndValueMismatchPromotions))]
    public void SuitMismatchTakesPriority(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => stack.Promote(nextCard));
        Assert.StartsWith("Card suit mismatch", exception.Message);
    }

    public static PromotionTestData StackFullPromotions {
        get {
            var data = new PromotionTestData();
            foreach (var suit in Enum.GetValues<Suit>()) {
                for (var i = 1; i < MinorArcana.MaxValue; i++) {
                    data.Add(new MinorArcana(suit, MinorArcana.MaxValue), new MinorArcana(suit, i));
                }
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(StackFullPromotions))]
    public void CannotPromoteToFullStack(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Assert
        Assert.False(stack.CanPromote(nextCard));
    }

    [Theory]
    [MemberData(nameof(StackFullPromotions))]
    public void StackFullPromotionCausesException(MinorArcana topCard, MinorArcana nextCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => stack.Promote(nextCard));
        Assert.Equal("Card stack full", exception.Message);
    }
}
using FluentAssertions;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class MinorArcanaStackTests {
    public static IEnumerable<object[]> CanAscendTestData {
        get {
            foreach (var valid in ValidAscensions) yield return valid.Append(true).ToArray();
            foreach (var invalid in InvalidAscensions) yield return invalid.Append(false).ToArray();
        }
    }

    public static IEnumerable<object[]> ValidAscensions {
        get {
            yield return new object[] { new MinorArcana(Suit.Coins, 3), new MinorArcana(Suit.Coins, 4) };
        }
    }

    public static IEnumerable<object[]> InvalidAscensions {
        get {
            yield return new object[] { new MinorArcana(Suit.Coins, 3), new MinorArcana(Suit.Goblets, 4) };
            yield return new object[] { new MinorArcana(Suit.Coins, 3), new MinorArcana(Suit.Coins, 2) };
            yield return new object[] { new MinorArcana(Suit.Coins, 3), new MinorArcana(Suit.Coins, 3) };
        }
    }

    [Theory]
    [MemberData(nameof(CanAscendTestData))]
    public void CanAscendTest(MinorArcana topCard, MinorArcana testedCard, bool expectedResult) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);

        // Assert
        stack.CanAscend(testedCard).Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(InvalidAscensions))]
    public void InvalidAscensionThrows(MinorArcana topCard, MinorArcana ascendedCard) {
        // Arrange
        var stack = new MinorArcanaStack(topCard);
        var message = ascendedCard.Suit == topCard.Suit ? "Card value ineligible for ascension *" : "Card suit mismatch *";

        // Act
        var action = () => stack.Ascend(ascendedCard);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage(message);
    }
}
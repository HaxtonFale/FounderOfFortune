using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using PromotionTestData = Xunit.TheoryData<FounderOfFortune.Game.Model.MajorArcana?, FounderOfFortune.Game.Model.MajorArcana?, FounderOfFortune.Game.Model.MajorArcana>;

namespace FounderOfFortune.Game.Test.Collections;

public class MajorArcanaStacksTests
{
    private static IEnumerable<MajorArcana?> ValidStackValues
    {
        get {
            yield return null;
            for (var i = 0; i <= 21; i++)
            {
                yield return new MajorArcana(i);
            }
        }
    }

    private static IEnumerable<(MajorArcana?, MajorArcana?)> AllStackValuePairs
    {
        get {
            var cards = ValidStackValues.ToList();
            return cards.SelectMany(left => cards.Select(right => (left, right)));
        }
    }

    private static IEnumerable<(MajorArcana?, MajorArcana?)> ValidStackValuePairs => AllStackValuePairs.Where(pair => {
        var (left, right) = pair;
        return IsValidStackState(left, right);
    });

    [Fact]
    public void DefaultConstructorCreatesEmptyStacks()
    {
        // Arrange
        var stack = new MajorArcanaStacks();

        // Assert
        stack.Left.Should().BeNull();
        stack.Right.Should().BeNull();
    }

    public static PromotionTestData ValidLeftPromotions
    {
        get {
            var data = new PromotionTestData();
            foreach (var (left, right) in ValidStackValuePairs)
            {
                if (left == null)
                {
                    data.Add(left, right, new MajorArcana(0));
                }
                else if (left.Value < 21)
                {
                    data.Add(left, right, left + 1);
                }
            }
            return data;
        }
    }

    public static PromotionTestData ValidRightPromotions
    {
        get {
            var data = new PromotionTestData();
            foreach (var (left, right) in ValidStackValuePairs)
            {
                if (right == null)
                {
                    data.Add(left, right, new MajorArcana(21));
                }
                else if (right.Value > 0)
                {
                    data.Add(left, right, right - 1);
                }
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ValidLeftPromotions))]
    [MemberData(nameof(ValidRightPromotions))]
    public void CanPromoteValidCombinations(MajorArcana? left, MajorArcana? right, MajorArcana card)
    {
        // Arrange
        var stack = new MajorArcanaStacks(left, right);

        // Assert
        stack.CanPromote(card).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidLeftPromotions))]
    public void ValidPromotionsAdvanceLeftStack(MajorArcana? left, MajorArcana? right, MajorArcana card)
    {
        // Arrange
        var stack = new MajorArcanaStacks(left, right);

        // Act
        var newStack = stack.Promote(card);

        // Assert
        newStack.Left.Should().Be(card);
    }

    [Theory]
    [MemberData(nameof(ValidRightPromotions))]
    public void ValidPromotionsAdvanceRightStack(MajorArcana? left, MajorArcana? right, MajorArcana card)
    {
        // Arrange
        var stack = new MajorArcanaStacks(left, right);

        // Act
        var newStack = stack.Promote(card);

        // Assert
        newStack.Right.Should().Be(card);
    }
    public static TheoryData<MajorArcana, MajorArcana> DescendingPairs
    {
        get {
            var data = new TheoryData<MajorArcana, MajorArcana>();
            for (var left = 1; left <= 21; left++)
            {
                for (var right = 0; right < left; right++)
                {
                    data.Add(new MajorArcana(left), new MajorArcana(right));
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(DescendingPairs))]
    public void DescendingCardPairsCauseException(MajorArcana left, MajorArcana right)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new MajorArcanaStacks(left, right));
        Assert.StartsWith("Left cannot be greater than right", exception.Message);
    }

    public static TheoryData<MajorArcana, MajorArcana> InvalidCardDistancePairs
    {
        get {
            var data = new TheoryData<MajorArcana, MajorArcana>();
            for (var i = 0; i < 21; i++)
            {
                data.Add(new MajorArcana(i), new MajorArcana(i + 1));
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCardDistancePairs))]
    public void CardsDifferingByOneCauseException(MajorArcana left, MajorArcana right)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new MajorArcanaStacks(left, right));
        exception.Message.Should().StartWith("Stacks cannot differ by 1");
    }

    public static PromotionTestData InvalidPromotionCombinations
    {
        get {
            var data = new PromotionTestData();
            foreach (var (left, right) in ValidStackValuePairs)
            {
                var leftLimit = left?.Value ?? -1;
                var rightLimit = right?.Value ?? 22;

                for (var i = 0; i <= 21; i++)
                {
                    if (i == leftLimit + 1 || i == rightLimit - 1) continue;
                    data.Add(left, right, new MajorArcana(i));
                }
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(InvalidPromotionCombinations))]
    public void CannotPromoteInvalidCombinations(MajorArcana? left, MajorArcana? right, MajorArcana card)
    {
        // Arrange
        var stack = new MajorArcanaStacks(left, right);

        // Assert
        stack.CanPromote(card).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidPromotionCombinations))]
    public void InvalidPromotionsCauseExceptions(MajorArcana? left, MajorArcana? right, MajorArcana card)
    {
        // Arrange
        var stack = new MajorArcanaStacks(left, right);
        var act = () => stack.Promote(card);

        // Act & Assert
        act.Should().Throw<ArgumentException>().Which.Message.Should().StartWith("Card value ineligible for promotion");
    }

    #region Helpers

    private static bool IsValidStackState(MajorArcana? left, MajorArcana? right)
    {
        return (left, right) switch {
            (null, null) => true,
            ( { } lCard, null) => lCard.Value < 21,
            (null, { } rCard) => rCard.Value > 0,
            ( { } lCard, { } rCard) => rCard - lCard > 1
        };
    }

    #endregion
}
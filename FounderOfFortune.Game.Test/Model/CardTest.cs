using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Model;

public class CardTest {
    public static IEnumerable<object[]> EqualityTestData => new List<object[]>
    {
        new object[] {new MajorArcana(4), new MajorArcana(4), true},
        new object[] {new MajorArcana(4), new MajorArcana(3), false},
        new object[] {new MinorArcana(Suit.Coins, 4), new MajorArcana(3), false},
        new object[] {new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 3), false},
        new object[] {new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Goblets, 4), false},
        new object[] {new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 4), true}
    };

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void EqualityTest(Card left, Card right, bool expected) {
        Assert.Equal(expected, left.Equals(right));
    }
}
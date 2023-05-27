using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Model;

public class CardTests {
    public static IEnumerable<object[]> EqualityTestData {
        get {
            yield return new object[] { new MajorArcana(4), new MajorArcana(4), true };
            yield return new object[] { new MajorArcana(4), new MajorArcana(3), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MajorArcana(3), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 3), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Goblets, 4), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 4), true };
        }
    }

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void EqualityTest(Card left, Card right, bool expected) {
        Assert.Equal(expected, left.Equals(right));
    }

    public static IEnumerable<object[]> AdjacencyTestData {
        get {
            yield return new object[] { new MajorArcana(4), new MajorArcana(4), false };
            yield return new object[] { new MajorArcana(4), new MajorArcana(3), true };
            yield return new object[] { new MajorArcana(3), new MajorArcana(4), true };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MajorArcana(3), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 3), true };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Goblets, 4), false };
            yield return new object[] { new MinorArcana(Suit.Coins, 4), new MinorArcana(Suit.Coins, 4), false };
        }

    }

    [Theory]
    [MemberData(nameof(AdjacencyTestData))]
    public void AdjacencyTest(Card left, Card right, bool expected) {
        Assert.Equal(expected, left.IsAdjacentTo(right));
    }
}
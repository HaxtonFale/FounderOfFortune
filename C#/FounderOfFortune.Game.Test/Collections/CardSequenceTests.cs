using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class CardSequenceTests {
    public static IEnumerable<object[]> CountTestData {
        get {
            yield return new object[] { new MajorArcana(0), 0, 1 };
            yield return new object[] { new MajorArcana(1), 3, 3 };
            yield return new object[] { new MajorArcana(3), 1, 3 };
        }
    }

    [Theory]
    [MemberData(nameof(CountTestData))]
    public void CountTest(Card initialCard, int finalCardValue, int expectedCount) {
        // Arrange
        var sequence = new CardSequence(initialCard, finalCardValue);

        // Assert
        sequence.Count.Should().Be(expectedCount);
    }

    public static IEnumerable<object[]> IndexTestData {
        get {
            yield return new object[] { new MajorArcana(0), 0, 0, new MajorArcana(0) };
            yield return new object[] { new MajorArcana(1), 3, 1, new MajorArcana(2) };
            yield return new object[] { new MajorArcana(3), 0, 2, new MajorArcana(1) };
            yield return new object[] { new MinorArcana(Suit.Coins, 1), 3, 1, new MinorArcana(Suit.Coins, 2) };
        }
    }

    [Theory]
    [MemberData(nameof(IndexTestData))]
    public void IndexTest(Card initialCard, int finalCardValue, int index, Card expectedCard) {
        // Arrange
        var sequence = new CardSequence(initialCard, finalCardValue);

        // Assert
        sequence[index].Should().Be(expectedCard);
    }

    [Fact]
    private void EnumerationTest() {
        // Arrange
        var sequence = new CardSequence(new MajorArcana(3), 7);
        var expected = Enumerable.Range(3, 5).Select(i => new Card(new MajorArcana(i)));

        // Assert
        sequence.Should().BeEquivalentTo(expected);
    }
}
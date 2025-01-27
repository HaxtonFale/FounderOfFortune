using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Serialization;

namespace FounderOfFortune.Solver.Test.Serializer;

public class BoardSerializerTests
{
    public static TheoryData<MajorArcana> AllMajorArcana
    {
        get
        {
            var testData = new TheoryData<MajorArcana>();
            for (var i = 0; i <= 21; i++)
            {
                testData.Add(new MajorArcana(i));
            }
            return testData;
        }
    }

    [Theory]
    [MemberData(nameof(AllMajorArcana))]
    public void MajorArcanaSerializeCorrectly(MajorArcana card)
    {
        // Act
        var cardByte = BoardSerializer.CardToByte(card);

        // Assert
        cardByte.Should().Be((byte)(card.Value + 1));
    }

    public static TheoryData<byte> MajorArcanaBytes
    {
        get {
            var testData = new TheoryData<byte>();
            for (byte i = 1; i <= 22; i++)
            {
                testData.Add(i);
            }
            return testData;
        }
    }

    [Theory]
    [MemberData(nameof(MajorArcanaBytes))]
    public void MajorArcanaDeserializeCorrectly(byte cardByte)
    {
        // Act
        var card = BoardSerializer.ByteToCard(cardByte);

        // Assert
        card.Should().BeOfType<MajorArcana>();
        card.Value.Should().Be(cardByte - 1);
    }

    const int Offset = MajorArcana.MaxValue - MajorArcana.MinValue + 1;
    const int SuitLength = MinorArcana.MaxValue - MinorArcana.MinValue + 1;

    public static TheoryData<MinorArcana> AllMinorArcana
    {
        get {
            var testData = new TheoryData<MinorArcana>();
            foreach (var suit in Enum.GetValues<Suit>())
                for (var i = MinorArcana.MinValue; i <= MinorArcana.MaxValue; i++)
                    testData.Add(new MinorArcana(suit, i));
            return testData;
        }
    }

    [Theory]
    [MemberData(nameof(AllMinorArcana))]
    public void MinorArcanaSerializeCorrectly(MinorArcana card)
    {
        // Act
        var cardByte = BoardSerializer.CardToByte(card);

        // Assert
        cardByte.Should().Be((byte) (SuitLength * (int) card.Suit + card.Value + Offset));
    }

    public static TheoryData<byte> MinorArcanaBytes
    {
        get {
            var testData = new TheoryData<byte>();
            foreach (var suit in Enum.GetValues<Suit>())
                for (var i = MinorArcana.MinValue; i <= MinorArcana.MaxValue; i++)
                    testData.Add((byte) (SuitLength * (int)suit + Offset + i));
            return testData;
        }
    }

    [Theory]
    [MemberData(nameof(MinorArcanaBytes))]
    public void MinorArcanaDeserializeCorrectly(byte cardByte)
    {
        // Arrange
        var expectedSuit = (Suit) ((cardByte - Offset) / SuitLength);
        var expectedValue = (cardByte - Offset) % SuitLength + 1;

        // Act
        var card = BoardSerializer.ByteToCard(cardByte);

        // Assert
        card.Should().BeOfType<MinorArcana>().Which.Suit.Should().Be(expectedSuit);
        card.Value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(AllMajorArcana))]
    [MemberData(nameof(AllMinorArcana))]
    public void DeserializationReversesSerialization(Card card)
    {
        // Act
        var cardByte = BoardSerializer.CardToByte(card);
        var newCard = BoardSerializer.ByteToCard(cardByte);

        // Assert
        newCard.Should().Be(card);
    }
}
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Serialization;
using Microsoft.Extensions.Logging;
using Serilog;

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
    
    private readonly BoardSerializer _boardSerializer;

    public BoardSerializerTests()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .CreateLogger();
        var factory = new LoggerFactory()
            .AddSerilog(Log.Logger, true);
        _boardSerializer = new BoardSerializer(factory.CreateLogger<BoardSerializer>());
    }

    [Theory]
    [MemberData(nameof(AllMajorArcana))]
    public void MajorArcanaSerializeCorrectly(MajorArcana card)
    {
        // Act
        var cardByte = _boardSerializer.CardToByte(card);

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
        var card = _boardSerializer.ByteToCard(cardByte);

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
        var cardByte = _boardSerializer.CardToByte(card);

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
        var minorArcanaByte = cardByte - Offset - 1;
        var expectedValue = minorArcanaByte % SuitLength + 1;
        var expectedSuit = minorArcanaByte / SuitLength;
        var expectedCard = new MinorArcana((Suit) expectedSuit, expectedValue);

        // Act
        var card = _boardSerializer.ByteToCard(cardByte);

        // Assert
        card.Should().BeOfType<MinorArcana>().And.BeEquivalentTo(expectedCard);
    }

    [Theory]
    [MemberData(nameof(AllMajorArcana))]
    [MemberData(nameof(AllMinorArcana))]
    public void DeserializationReversesSerialization(Card card)
    {
        // Act
        var cardByte = _boardSerializer.CardToByte(card);
        var newCard = _boardSerializer.ByteToCard(cardByte);

        // Assert
        newCard.Should().Be(card);
    }

    public static TheoryData<byte> InvalidBytes
    {
        get
        {
            var testData = new TheoryData<byte> { 0 };
            const int minValue = 4 * MinorArcana.MaxValue + MajorArcana.MaxValue + 2;
            for (var i = minValue; i <= byte.MaxValue; i++)
            {
                testData.Add((byte)i);
            }
            return testData;
        }
    }

    [Theory]
    [MemberData(nameof(InvalidBytes))]
    public void InvalidBytesCauseException(byte cardByte)
    {
        // Arrange
        var act = () => _boardSerializer.ByteToCard(cardByte);

        // Act & Assert
        act.Should().Throw<InvalidDataException>();
    }
}
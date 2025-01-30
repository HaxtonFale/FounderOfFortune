using System.Collections.Immutable;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;
using Solver.Core.Serialization;

namespace FounderOfFortune.Solver.Serialization;

internal class BoardSerializer(ILogger<BoardSerializer> logger) : IStateSerializer<BoardState, GameAction>
{
    private const byte Store = 1;
    private const byte Retrieve = 2;
    private const byte Move = 3;

    public int SerializeStep(GameAction step, byte[] bytes)
    {
        switch (step)
        {
            case MoveCard m:
                bytes[0] = Move;
                bytes[1] = BitConverter.GetBytes(m.From)[0];
                bytes[2] = BitConverter.GetBytes(m.To)[0];
                break;
            case StoreCard s:
                bytes[0] = Store;
                bytes[1] = BitConverter.GetBytes(s.From)[0];
                break;
            case RetrieveCard r:
                bytes[0] = Retrieve;
                bytes[2] = BitConverter.GetBytes(r.To)[0];
                break;
        }
        return SerializedStepLength;
    }

    public int SerializeState(BoardState state, byte[] bytes)
    {
        var index = 0;
        foreach (var stack in state.TableauStacks)
        {
            foreach (var card in stack.Cards)
            {
                bytes[index++] = CardToByte(card);
            }

            bytes[index++] = 0;
        }

        if (state.FreeCell != null)
        {
            bytes[index++] = CardToByte(state.FreeCell);
        }
        else
        {
            bytes[index++] = 0;
        }

        if (state.MajorArcanaStacks.Left == state.MajorArcanaStacks.Right && state.MajorArcanaStacks.Left != null)
        {
            bytes[index++] = CardToByte(state.MajorArcanaStacks.Left);
        }
        else
        {
            bytes[index++] = 0;
        }
        
        return index;
    }

    public GameAction DeserializeStep(ReadOnlySpan<byte> buffer)
    {
        var from = (int) buffer[1];
        var to = (int) buffer[2];
        return buffer[0] switch
        {
            Move => new MoveCard(from, to),
            Store => new StoreCard(from),
            Retrieve => new RetrieveCard(to),
            _ => throw new InvalidDataException($"Cannot deserialize byte {buffer[0]:X} into a valid step.")
        };
    }

    public BoardState DeserializeState(ReadOnlySpan<byte> buffer)
    {
        var tableau = ImmutableList<TableauStack>.Empty;
        var bufPointer = 0;
        for (var i = 0; i < 11; i++)
        {
            logger.LogTrace("Reading stack {Stack}", i + 1);
            var currentStack = new List<Card>();
            var currentByte = buffer[bufPointer++];
            while (currentByte != 0)
            {
                currentStack.Add(ByteToCard(currentByte));
                currentByte = buffer[bufPointer++];
            }
            tableau = tableau.Add(new TableauStack(currentStack.ToImmutableList()));
        }

        var freeCell = buffer[bufPointer] == 0 ? null : ByteToCard(buffer[bufPointer]);
        bufPointer++;

        var finalPromotedMajor = buffer[bufPointer] == 0 ? null : (MajorArcana)ByteToCard(buffer[bufPointer]);

        var cardsInPlay = tableau.SelectMany(s => s.Cards).ToHashSet();
        MajorArcanaStacks majorStacks;
        if (finalPromotedMajor != null)
        {
            majorStacks = new MajorArcanaStacks(finalPromotedMajor, finalPromotedMajor);
        }
        else
        {
            var majorArcanaInPlay = cardsInPlay.Where(c => c is MajorArcana).Cast<MajorArcana>().ToHashSet();
            var majorLeft = majorArcanaInPlay.Min()!;
            var majorRight = majorArcanaInPlay.Max()!;
            majorStacks = new MajorArcanaStacks(majorLeft.Value == MajorArcana.MinValue ? null : majorLeft - 1,
                majorRight.Value == MajorArcana.MaxValue ? null : majorRight + 1);
        }

        var minorStacks = cardsInPlay.Where(c => c is MinorArcana).Cast<MinorArcana>().GroupBy(c => c.Suit)
            .ToImmutableDictionary(g => g.Key, g => new MinorArcanaStack(g.Min()! - 1));

        return new BoardState(majorStacks, new MinorArcanaStacks(minorStacks), freeCell, tableau);
    }

    public int SerializedStepLength => 3;

    private const int MajorArcanaLength = MajorArcana.MaxValue - MajorArcana.MinValue + 1;
    private const int MinorArcanaSuitLength = MinorArcana.MaxValue - MinorArcana.MinValue + 1;
    private const int MaxCardByteValue = MajorArcanaLength + MinorArcanaSuitLength * 4;

    public byte CardToByte(Card card)
    {
        byte b;
        if (card is MajorArcana major)
        {
            logger.LogTrace("Card is a Major Arcana ({Card})", card);
            b = (byte) (major.Value + 1);
        } else if (card is MinorArcana minor)
        {
            logger.LogTrace("Card is a Minor Arcana ({Card})", card);
            var suit = (int) minor.Suit;
            logger.LogTrace("Suit value: {suit}", suit);
            b = (byte)(suit * MinorArcanaSuitLength + minor.Value + MajorArcanaLength);
        }
        else
        {
            throw new ArgumentException("Not a valid card type", nameof(card));
        }
        logger.LogTrace("Card {Card} written as byte {Byte:X2}", card, b);
        return b;
    }

    public Card ByteToCard(byte b)
    {
        if (b is 0 or > MaxCardByteValue) throw new InvalidDataException($"Cannot convert byte {b} to card.");

        logger.LogTrace("Byte {Byte:X2} read as Card", b);
        Card card;
        if (b <= MajorArcanaLength)
        {
            var majorArcanaValue = b - 1;
            logger.LogTrace("Card is a Major Arcana with value {Value}", majorArcanaValue);
            card = new MajorArcana(majorArcanaValue);
        }
        else
        {
            var minorArcanaValue = b - MajorArcanaLength - 1;
            var value = minorArcanaValue % MinorArcanaSuitLength + 1;
            var suit = minorArcanaValue / MinorArcanaSuitLength;
            logger.LogTrace("Card is a Minor Arcana with value {Value}", value);
            card = new MinorArcana((Suit) suit, value);
        }

        logger.LogTrace("Byte {Byte:X2} read as card {Card}", b, card);
        return card;
    }
}
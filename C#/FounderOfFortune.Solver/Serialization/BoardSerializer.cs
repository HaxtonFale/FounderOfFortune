using FounderOfFortune.Game;
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Model;
using Solver.Core.Serialization;

namespace FounderOfFortune.Solver.Serialization;

public class BoardSerializer : IStateSerializer<BoardState, GameAction>
{
    private const byte Move = 3;
    private const byte Store = 1;
    private const byte Retrieve = 2;

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
        // 11 terminators + final card + the number of cards on the board
        var bufferSize = 12 + state.TableauStacks.Select(s => s.Cards.Count).Sum();
        var index = 0;
        foreach (var stack in state.TableauStacks)
        {
            foreach (var card in stack.Cards)
            {
                bytes[index++] = CardToByte(card);
            }

            bytes[index++] = 0;
        }

        if (state.MajorArcanaStacks.Left == state.MajorArcanaStacks.Right && state.MajorArcanaStacks.Left.HasValue)
        {
            bytes[index] = CardToByte(state.MajorArcanaStacks.Left.Value);
        }
        else
        {
            bytes[index] = 0;
        }
        
        return bufferSize;
    }

    public GameAction DeserializeStep(ReadOnlySpan<byte> buffer)
    {
        var from = (int) buffer[1];
        var to = (int)buffer[2];
        return buffer[0] switch
        {
            Move => new MoveCard(from, to),
            Store => new StoreCard(from),
            Retrieve => new RetrieveCard(to),
            _ => throw new InvalidDataException($"Cannot deserialize byte {buffer[0]} into a valid step.")
        };
    }

    public BoardState DeserializeState(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public int SerializedStepLength => 3;

    private static byte CardToByte(Card card)
    {
        if (card.IsMajorArcana)
        {
            var major = card.AsMajorArcana;
            return (byte) (major.Value + 1);
        }
        else
        {
            var offset = MajorArcana.MaxValue + 1;
            const int suitLength = MinorArcana.MaxValue - MinorArcana.MinValue + 1;
            var minor = card.AsMinorArcana;
            return (byte) ((int) minor.Suit * suitLength + card.Value + offset);
        }
    }

    private static Card ByteToCard(byte b)
    {
        throw new NotImplementedException();
    }
}
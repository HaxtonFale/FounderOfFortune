namespace FounderOfFortune.Game.Model;

public readonly struct MinorArcana : IComparable<MinorArcana>, IComparable
{
    public Suit Suit { get; }
    public int Value { get; }

    public MinorArcana(Suit suit, int value)
    {
        if (value is < 1 or > 13)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 13");
        Suit = suit;
        Value = value;
    }

    public override string ToString()
    {
        var number = Value switch
        {
            1 => "Ace",
            13 => "King",
            12 => "Queen",
            11 => "Jack",
            _ => Value.ToString()
        };
        return $"{number} of {Suit}";
    }

    public int CompareTo(MinorArcana other)
    {
        if (other.Suit != Suit)
            throw new ArgumentException("Cards must be of the same suit");
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is MinorArcana other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(MinorArcana)}");
    }

    public static bool operator <(MinorArcana left, MinorArcana right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(MinorArcana left, MinorArcana right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(MinorArcana left, MinorArcana right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(MinorArcana left, MinorArcana right)
    {
        return left.CompareTo(right) >= 0;
    }
}
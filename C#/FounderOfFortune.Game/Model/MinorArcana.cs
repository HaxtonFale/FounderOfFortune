namespace FounderOfFortune.Game.Model;

/// <summary>
/// Represents one of the minor arcana: cards valued from 1 to 13 (with Jack, Queen and King as the last three values) with one of the four <see cref="Suit"/>s.
/// </summary>
public class MinorArcana : Card, IEquatable<MinorArcana>, IComparable<MinorArcana>, IComparable
{
    public const int MinValue = 1;
    public const int MaxValue = 13;

    public Suit Suit { get; }
    public override int Value { get; }

    public MinorArcana(Suit suit, int value) {
        if (value is < MinValue or > MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 13");
        Suit = suit;
        Value = value;
    }

    public override string ToString() {
        var number = Value switch {
            1 => "Ace",
            13 => "King",
            12 => "Queen",
            11 => "Jack",
            _ => Value.ToString()
        };
        return $"{number} of {Suit}";
    }

    public override string FullName => ToString();

    public override bool IsAdjacentTo(Card other) => other is MinorArcana minor && minor.Suit == Suit && Math.Abs(minor.Value - Value) == 1;

    protected override Card ChangeBy(int offset) => new MinorArcana(Suit, Value + offset);

    #region Arithmetic operators

    public static MinorArcana operator +(MinorArcana card, int change) => new(card.Suit, card.Value + change);
    public static MinorArcana operator -(MinorArcana card, int change) => new(card.Suit, card.Value - change);

    #endregion

    #region Equality members

    public bool Equals(MinorArcana? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Suit == other.Suit && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((MinorArcana) obj);
    }

    public override int GetHashCode() => HashCode.Combine((int) Suit, Value);

    public static bool operator ==(MinorArcana? left, MinorArcana? right) => Equals(left, right);

    public static bool operator !=(MinorArcana? left, MinorArcana? right) => !Equals(left, right);

    #endregion

    #region Relational members

    public int CompareTo(MinorArcana? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        var suitComparison = Suit.CompareTo(other.Suit);
        return suitComparison != 0 ? suitComparison : Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;

        return obj is MinorArcana other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(MinorArcana)}");
    }

    public static bool operator <(MinorArcana? left, MinorArcana? right) => Comparer<MinorArcana>.Default.Compare(left, right) < 0;

    public static bool operator >(MinorArcana? left, MinorArcana? right) => Comparer<MinorArcana>.Default.Compare(left, right) > 0;

    public static bool operator <=(MinorArcana? left, MinorArcana? right) => Comparer<MinorArcana>.Default.Compare(left, right) <= 0;

    public static bool operator >=(MinorArcana? left, MinorArcana? right) => Comparer<MinorArcana>.Default.Compare(left, right) >= 0;

    #endregion
}
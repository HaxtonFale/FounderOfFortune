using Humanizer;

namespace FounderOfFortune.Game.Model;

/// <summary>
/// Represents one of the major arcana: named cards valued from 0 to 21.
/// </summary>
public class MajorArcana : Card, IEquatable<MajorArcana>, IComparable<MajorArcana>, IComparable
{
    private static readonly List<string> Names = new()
    {
        "The Fool",
        "The Magician",
        "The Priestess",
        "The Empress",
        "The Emperor",
        "The Hierophant",
        "The Lovers",
        "The Chariot",
        "Strength",
        "The Hermit",
        "The Wheel",
        "Justice",
        "The Hanged Man",
        "Death",
        "Temperance",
        "The Devil",
        "The Tower",
        "The Stars",
        "The Moon",
        "The Sun",
        "Judgement",
        "The World"
    };

    public const int MinValue = 0;
    public const int MaxValue = 21;

    public override int Value { get; }

    public MajorArcana(int value) {
        if (value is < 0 or > 21) throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 21.");
        Value = value;
    }

    public override string FullName => $"{this} - {Names[Value]}";

    public override string ToString() => Value == 0 ? "0" : Value.ToRoman();

    public override bool IsAdjacentTo(Card other) => other is MajorArcana major && Math.Abs(major.Value - Value) == 1;

    #region Arithmetic operators

    public static MajorArcana operator +(MajorArcana card, int change) => new(card.Value + change);
    public static MajorArcana operator -(MajorArcana card, int change) => new(card.Value - change);
    public static int operator -(MajorArcana card, MajorArcana other) => card.Value - other.Value;

    #endregion

    #region Equality members

    public bool Equals(MajorArcana? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((MajorArcana) obj);
    }

    public override int GetHashCode() => Value;

    protected override Card ChangeBy(int offset) => new MajorArcana(Value + offset);

    public static bool operator ==(MajorArcana? left, MajorArcana? right) => Equals(left, right);

    public static bool operator !=(MajorArcana? left, MajorArcana? right) => !Equals(left, right);

    #endregion

    #region Relational members

    public int CompareTo(MajorArcana? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        return other is null ? 1 : Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (ReferenceEquals(this, obj)) return 0;

        return obj is MajorArcana other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(MajorArcana)}");
    }

    public static bool operator <(MajorArcana? left, MajorArcana? right)
    {
        return Comparer<MajorArcana>.Default.Compare(left, right) < 0;
    }

    public static bool operator >(MajorArcana? left, MajorArcana? right)
    {
        return Comparer<MajorArcana>.Default.Compare(left, right) > 0;
    }

    public static bool operator <=(MajorArcana? left, MajorArcana? right)
    {
        return Comparer<MajorArcana>.Default.Compare(left, right) <= 0;
    }

    public static bool operator >=(MajorArcana? left, MajorArcana? right)
    {
        return Comparer<MajorArcana>.Default.Compare(left, right) >= 0;
    }

    #endregion
}
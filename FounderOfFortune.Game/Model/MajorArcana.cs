using Humanizer;

namespace FounderOfFortune.Game.Model;

public readonly struct MajorArcana : IComparable<MajorArcana>, IComparable
{
    public int Value { get; }

    public MajorArcana(int value) {
        if (value is < 0 or > 21) throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 21.");
        Value = value;
    }

    public override string ToString() {
        return Value != 0 ? Value.ToRoman() : "0";
    }

    public int CompareTo(MajorArcana other)
    {
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is MajorArcana other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(MajorArcana)}");
    }

    public static bool operator <(MajorArcana left, MajorArcana right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(MajorArcana left, MajorArcana right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(MajorArcana left, MajorArcana right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(MajorArcana left, MajorArcana right)
    {
        return left.CompareTo(right) >= 0;
    }
}
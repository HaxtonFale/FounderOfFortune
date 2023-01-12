using OneOf;

namespace FounderOfFortune.Game.Model;

public readonly struct Card : IEquatable<Card> {
    private readonly OneOf<MajorArcana, MinorArcana> _card;
    public bool IsMajorArcana => _card.IsT0;
    public bool IsMinorArcana => _card.IsT1;

    public MajorArcana AsMajorArcana => _card.AsT0;
    public MinorArcana AsMinorArcana => _card.AsT1;

    public Card(MajorArcana card) {
        _card = card;
    }

    public Card(MinorArcana card) {
        _card = card;
    }

    public bool IsAdjacentTo(Card other) {
        if (IsMajorArcana && other.IsMajorArcana) {
            return Math.Abs(AsMajorArcana.Value - other.AsMajorArcana.Value) == 1;
        }

        if (IsMinorArcana && other.IsMinorArcana) {
            return AsMinorArcana.Suit == other.AsMinorArcana.Suit && Math.Abs(AsMinorArcana.Value - other.AsMinorArcana.Value) == 1;
        }

        return false;
    }

    public override string ToString() => IsMajorArcana ? AsMajorArcana.ToString() : AsMinorArcana.ToString();

    #region IEquatable<T>

    public bool Equals(Card other) {
        if (IsMajorArcana && other.IsMajorArcana) return AsMajorArcana.Equals(other.AsMajorArcana);
        if (IsMinorArcana && other.IsMinorArcana) return AsMinorArcana.Equals(other.AsMinorArcana);
        return false;
    }

    public override bool Equals(object? obj) => obj is Card other && Equals(other);

    public override int GetHashCode() => _card.GetHashCode();

    public static bool operator ==(Card left, Card right) => left.Equals(right);

    public static bool operator !=(Card left, Card right) => !left.Equals(right);

    #endregion

    #region Conversion operators

    public static implicit operator Card(MajorArcana major) => new(major);
    public static implicit operator Card(MinorArcana minor) => new(minor);

    #endregion
}
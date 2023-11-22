using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStack(MinorArcana topCard) : IEquatable<MinorArcanaStack>
{
    public readonly MinorArcana TopCard = topCard;

    public MinorArcanaStack Promote(MinorArcana card)
    {
        if (card.Suit != TopCard.Suit)
        {
            throw new ArgumentException("Card suit mismatch", nameof(card));
        }

        if (TopCard.Value >= MinorArcana.MaxValue)
        {
            throw new InvalidOperationException("Card stack full");
        }
        if (card != TopCard + 1)
        {
            throw new ArgumentException("Card value ineligible for promotion", nameof(card));
        }

        return new MinorArcanaStack(card);
    }

    public bool CanPromote(MinorArcana card) => TopCard.Value < MinorArcana.MaxValue && card == TopCard + 1;

    #region IEquatable

    public bool Equals(MinorArcanaStack? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return TopCard.Equals(other.TopCard);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        return obj is MinorArcanaStack stack && Equals(stack);
    }

    public override int GetHashCode() => TopCard.GetHashCode();

    public static bool operator ==(MinorArcanaStack? left, MinorArcanaStack? right) => Equals(left, right);

    public static bool operator !=(MinorArcanaStack? left, MinorArcanaStack? right) => !Equals(left, right);

    #endregion
}
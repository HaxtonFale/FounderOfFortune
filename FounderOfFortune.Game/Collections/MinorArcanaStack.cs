using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStack : IEquatable<MinorArcanaStack> {
    public readonly MinorArcana TopCard;

    public MinorArcanaStack(MinorArcana topCard) => TopCard = topCard;

    public MinorArcanaStack Ascend(MinorArcana card) {
        if (card.Suit != TopCard.Suit) {
            throw new InvalidOperationException("Card suit mismatch");
        }
        if (card != TopCard + 1) {
            throw new InvalidOperationException("Card value ineligible for ascension");
        }

        return new MinorArcanaStack(card);
    }

    public bool CanAscend(MinorArcana card) => card.Suit == TopCard.Suit && card == TopCard + 1;

    #region IEquatable

    public bool Equals(MinorArcanaStack? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return TopCard.Equals(other.TopCard);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MinorArcanaStack)obj);
    }

    public override int GetHashCode() => TopCard.GetHashCode();

    public static bool operator ==(MinorArcanaStack? left, MinorArcanaStack? right) => Equals(left, right);

    public static bool operator !=(MinorArcanaStack? left, MinorArcanaStack? right) => !Equals(left, right);

    #endregion
}
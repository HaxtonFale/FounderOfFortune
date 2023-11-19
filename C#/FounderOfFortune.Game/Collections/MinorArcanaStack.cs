using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStack {
    public readonly MinorArcana TopCard;

    public MinorArcanaStack(MinorArcana topCard) {
        TopCard = topCard;
    }

    public MinorArcanaStack Promote(MinorArcana card) {
        if (card.Suit != TopCard.Suit) {
            throw new ArgumentException("Card suit mismatch", nameof(card));
        }

        if (TopCard.Value >= MinorArcana.MaxValue) {
            throw new InvalidOperationException("Card stack full");
        }
        if (card != TopCard + 1) {
            throw new ArgumentException("Card value ineligible for promotion", nameof(card));
        }

        return new MinorArcanaStack(card);
    }

    public bool CanPromote(MinorArcana card) => TopCard.Value < MinorArcana.MaxValue && card == TopCard + 1;
}
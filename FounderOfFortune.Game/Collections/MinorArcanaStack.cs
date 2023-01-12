using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStack {
    public readonly MinorArcana TopCard;

    public MinorArcanaStack(MinorArcana topCard) {
        TopCard = topCard;
    }

    public MinorArcanaStack Ascend(MinorArcana card) {
        if (card.Suit != TopCard.Suit) {
            throw new ArgumentException("Card suit mismatch", nameof(card));
        }
        if (card.Value != TopCard.Value + 1) {
            throw new ArgumentException("Card value ineligible for ascension", nameof(card));
        }

        return new MinorArcanaStack(card);
    }
}
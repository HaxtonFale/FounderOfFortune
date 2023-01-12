using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStack {
    private readonly Suit _suit;
    private readonly int _topCard = 1;

    public MinorArcanaStack(Suit suit) {
        _suit = suit;
    }

    private MinorArcanaStack(Suit suit, int topCard)
    {
        _suit = suit;
        _topCard = topCard;
    }
    
    public bool TryAscend(MinorArcana card, out MinorArcanaStack stack) {
        if (card.Suit != _suit || card.Value != _topCard + 1)
        {
            stack = this;
            return false;
        }

        stack = new MinorArcanaStack(_suit, _topCard + 1);
        return true;
    }
}
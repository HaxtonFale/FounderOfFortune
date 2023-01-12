using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class TableauStack {
    public readonly ImmutableList<Card> Cards;

    public TableauStack() : this(ImmutableList<Card>.Empty) {
    }

    public TableauStack(ImmutableList<Card> cards) {
        Cards = cards;
    }

    public ImmutableList<Card> TakeCards(out TableauStack stack) {
        if (Cards.IsEmpty) {
            stack = this;
            return ImmutableList<Card>.Empty;
        }

        var output = ImmutableList<Card>.Empty;
        var count = Cards.Count - 1;
        do {
            output = output.Add(Cards[count--]);
        } while (count >= 0 && Cards[count].IsAdjacent(output.Last()));

        var newCards = Cards.Take(count + 1);
        stack = new TableauStack(newCards.ToImmutableList());
        return output;
    }
}
using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class TableauStack {
    public readonly ImmutableList<Card> Cards;
    public readonly Card? TopCard;

    public bool IsEmpty => !TopCard.HasValue;

    public TableauStack() : this(ImmutableList<Card>.Empty) {
    }

    public TableauStack(ImmutableList<Card> cards) {
        Cards = cards;
        if (cards.Count == 0) { TopCard = null; }
        else { TopCard = cards.Last(); }
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
        } while (count >= 0 && Cards[count].IsAdjacentTo(output.Last()));
        
        stack = new TableauStack(Cards.Take(count + 1).ToImmutableList());
        return output;
    }

    public Card TakeCard(out TableauStack stack)
    {
        if (Cards.IsEmpty) throw new InvalidOperationException("Cannot take a card from empty stack");
        stack = new TableauStack(Cards.SkipLast(1).ToImmutableList());
        return Cards.Last();
    }

    public TableauStack PlaceCard(Card card) {
        if (TopCard == null || TopCard.Value.IsAdjacentTo(card)) {
            return new TableauStack(Cards.Add(card));
        }

        throw new ArgumentException($"Cannot place card on top of stack ({Cards.Last()}", nameof(card));
    }

    public TableauStack PlaceRange(IEnumerable<Card> cards)
    {
        return cards.Aggregate(this, (current, card) => current.PlaceCard(card));
    }
}
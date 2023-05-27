using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

/// <summary>
/// Represents a single stack (of 11) on the tableau.
/// While it begins with an arbitrary assortment of cards, it will only accept cards if it's empty, or if the first incoming card can be placed on the one on the top.
/// </summary>
public class TableauStack {
    public readonly ImmutableList<Card> Cards;
    public readonly Card? TopCard;

    /// <summary>
    /// Initialize an empty stack.
    /// </summary>
    public TableauStack() : this(ImmutableList<Card>.Empty) {
    }

    public TableauStack(ImmutableList<Card> cards) {
        Cards = cards;
        if (cards.Count == 0) { TopCard = null; }
        else { TopCard = cards.Last(); }
    }

    /// <summary>
    /// Take a sequence of consecutive cards from the top of the stack.
    /// </summary>
    /// <param name="stack">New instance of <see cref="TableauStack"/> with the remaining cards (if any).</param>
    /// <exception cref="InvalidOperationException">Thrown when called on an empty stack.</exception>
    public CardSequence TakeCards(out TableauStack stack) {
        if (Cards.IsEmpty) {
            throw new InvalidOperationException("Cannot take cards from an empty stack");
        }

        var index = Cards.Count - 1;
        var initialCard = Cards[index];
        var finalCard = initialCard;
        while (index > 0 && Cards[index - 1].IsAdjacentTo(finalCard))
        {
            finalCard = Cards[--index];
        }

        stack = new TableauStack(Cards.Take(index).ToImmutableList());
        return new CardSequence(initialCard, finalCard.Value);
    }

    /// <summary>
    /// Take a single card from the top of the stack.
    /// </summary>
    /// <param name="stack">New instance of <see cref="TableauStack"/> with the remaining cards (if any).</param>
    /// <exception cref="InvalidOperationException">Thrown when called on an empty stack.</exception>
    public Card TakeCard(out TableauStack stack)
    {
        if (Cards.IsEmpty) throw new InvalidOperationException("Cannot take a card from empty stack");
        stack = new TableauStack(Cards.SkipLast(1).ToImmutableList());
        return Cards.Last();
    }

    /// <summary>
    /// Place a new card on top of the stack.
    /// </summary>
    /// <param name="card">The card to be placed.</param>
    /// <returns>Stack updated with the new card.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to place an invalid card.</exception>
    public TableauStack PlaceCard(Card card) {
        if (TopCard == null || TopCard.Value.IsAdjacentTo(card)) {
            return new TableauStack(Cards.Add(card));
        }

        throw new InvalidOperationException($"Cannot place {card} on top of stack ({Cards.Last()})");
    }

    /// <summary>
    /// Place a sequence of cards on top of the stack.
    /// </summary>
    /// <param name="cards">The cards to be placed.</param>
    /// <returns>Stack updated with the new cards.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to place an invalid card.</exception>
    public TableauStack PlaceRange(IEnumerable<Card> cards)
    {
        return cards.Aggregate(this, (current, card) => current.PlaceCard(card));
    }
}
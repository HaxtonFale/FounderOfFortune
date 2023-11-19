using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStacks
{
    private readonly ImmutableDictionary<Suit, MinorArcanaStack> _stacks;

    public IEnumerable<MinorArcanaStack> Stacks => _stacks.Values;

    public MinorArcanaStacks()
    {
        _stacks = ImmutableDictionary<Suit, MinorArcanaStack>.Empty
            .Add(Suit.Coins, new MinorArcanaStack(new MinorArcana(Suit.Coins, 1)))
            .Add(Suit.Goblets, new MinorArcanaStack(new MinorArcana(Suit.Goblets, 1)))
            .Add(Suit.Swords, new MinorArcanaStack(new MinorArcana(Suit.Swords, 1)))
            .Add(Suit.Wands, new MinorArcanaStack(new MinorArcana(Suit.Wands, 1)));
    }

    private MinorArcanaStacks(ImmutableDictionary<Suit, MinorArcanaStack> stacks)
    {
        _stacks = stacks;
    }

    public MinorArcanaStacks Promote(MinorArcana card)
    {
        var stack = _stacks[card.Suit];
        var newStack = stack.Promote(card);
        return new MinorArcanaStacks(_stacks.SetItem(card.Suit, newStack));
    }

    public MinorArcana TopCard(Suit suit) => _stacks[suit].TopCard;

    public bool CanPromote(MinorArcana card) => _stacks[card.Suit].CanPromote(card);

    public MinorArcanaStacks PromoteRange(IEnumerable<MinorArcana> cards) => cards.Aggregate(this, (stacks, card) => stacks.Promote(card));
}
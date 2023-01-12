using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStacks {
    private readonly ImmutableDictionary<Suit, MinorArcanaStack> _stacks;

    public MinorArcanaStacks() {
        _stacks = ImmutableDictionary<Suit, MinorArcanaStack>.Empty
            .Add(Suit.Coins, new MinorArcanaStack(Suit.Coins))
            .Add(Suit.Goblets, new MinorArcanaStack(Suit.Goblets))
            .Add(Suit.Swords, new MinorArcanaStack(Suit.Swords))
            .Add(Suit.Wands, new MinorArcanaStack(Suit.Wands));
    }

    private MinorArcanaStacks(ImmutableDictionary<Suit, MinorArcanaStack> stacks) {
        _stacks = stacks;
    }

    public bool TryAscend(MinorArcana card, out MinorArcanaStacks stacks) {
        var stack = _stacks[card.Suit];
        if (stack.TryAscend(card, out var newStack)) {
            stacks = new MinorArcanaStacks(_stacks.SetItem(card.Suit, newStack));
            return true;
        }

        stacks = this;
        return false;
    }
}
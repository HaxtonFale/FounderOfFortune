using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStacks : IEquatable<MinorArcanaStacks>
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

    #region IEquatable

    public bool Equals(MinorArcanaStacks? other)
    {
        if (other is null) return false;

        return ReferenceEquals(this, other) || Enum.GetValues<Suit>().All(suit => _stacks[suit].Equals(other._stacks[suit]));
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        return obj is MinorArcanaStacks stacks && Equals(stacks);
    }

    public override int GetHashCode() => _stacks.GetHashCode();

    public static bool operator ==(MinorArcanaStacks? left, MinorArcanaStacks? right) => Equals(left, right);

    public static bool operator !=(MinorArcanaStacks? left, MinorArcanaStacks? right) => !Equals(left, right);

    #endregion
}
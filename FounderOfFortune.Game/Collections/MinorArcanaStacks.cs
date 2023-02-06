using System.Collections.Immutable;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MinorArcanaStacks : IEquatable<MinorArcanaStacks> {
    private readonly ImmutableDictionary<Suit, MinorArcanaStack> _stacks;

    public MinorArcanaStacks() =>
        _stacks = ImmutableDictionary<Suit, MinorArcanaStack>.Empty
            .Add(Suit.Coins, new MinorArcanaStack(new MinorArcana(Suit.Coins, 1)))
            .Add(Suit.Goblets, new MinorArcanaStack(new MinorArcana(Suit.Goblets, 1)))
            .Add(Suit.Swords, new MinorArcanaStack(new MinorArcana(Suit.Swords, 1)))
            .Add(Suit.Wands, new MinorArcanaStack(new MinorArcana(Suit.Wands, 1)));

    private MinorArcanaStacks(ImmutableDictionary<Suit, MinorArcanaStack> stacks) => _stacks = stacks;

    public MinorArcanaStacks Ascend(MinorArcana card) {
        var stack = _stacks[card.Suit];
        var newStack = stack.Ascend(card);
        return new MinorArcanaStacks(_stacks.SetItem(card.Suit, newStack));
    }

    public MinorArcana TopCard(Suit suit) => _stacks[suit].TopCard;

    public bool CanAscend(MinorArcana card) => _stacks[card.Suit].CanAscend(card);

    public MinorArcanaStacks AscendRange(IEnumerable<MinorArcana> cards) => cards.Aggregate(this, (stacks, card) => stacks.Ascend(card));

    #region IEquatable

    public bool Equals(MinorArcanaStacks? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Enum.GetValues<Suit>().All(s => _stacks[s] == other._stacks[s]);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MinorArcanaStacks)obj);
    }

    public override int GetHashCode() => _stacks.GetHashCode();

    public static bool operator ==(MinorArcanaStacks? left, MinorArcanaStacks? right) => Equals(left, right);

    public static bool operator !=(MinorArcanaStacks? left, MinorArcanaStacks? right) => !Equals(left, right);

    #endregion
}
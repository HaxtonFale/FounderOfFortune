using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

/// <summary>
/// Represents the pair of major arcana stacks on the board.
/// The stacks are a pair of slots that start empty but can take cards on either end until they meet in the middle.
/// </summary>
/// <remarks>
/// Created as an immutable collection to make backtracking easier.
/// </remarks>
public class MajorArcanaStacks : IEquatable<MajorArcanaStacks> {
    private readonly int _left;
    /// <summary>
    /// The lower end of the stack.<br />
    /// When empty, it accepts 0; otherwise, it will only take a card whose value is higher than it by 1.
    /// </summary>
    public readonly MajorArcana? LeftCard;

    private readonly int _right;
    /// <summary>
    /// The higher end of the stack.<br />
    /// When empty, it accepts 21; otherwise, it will only take a card whose value is lower than it by 1.
    /// </summary>
    public readonly MajorArcana? RightCard;

    /// <summary>
    /// Initializes a pair of stacks into empty state.
    /// </summary>
    public MajorArcanaStacks() : this(-1, 22) {
    }

    /// <summary>
    /// Initializes a pair of stacks into the specified state.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="left"/> is greater than <paramref name="right"/>.</exception>
    public MajorArcanaStacks(int left, int right) {
        if (left < -1) throw new ArgumentOutOfRangeException(nameof(left), "Left cannot be lower than -1 (empty stack)");
        if (right > 22) throw new ArgumentOutOfRangeException(nameof(right), "Right cannot be greater than 22 (empty stack)");
        if (left > right) throw new ArgumentException("Left cannot be greater than right.");
        _left = left;
        LeftCard = _left > -1 ? new MajorArcana(_left) : null;

        _right = right;
        RightCard = _right < 22 ? new MajorArcana(_right) : null;
    }

    /// <summary>
    /// Ascends a card to the stacks.
    /// </summary>
    /// <param name="card">The major arcana to ascend. Must be eligible for ascension, i.e. greater by 1 than <see cref="LeftCard"/> or lower by 1 than <see cref="RightCard"/>.</param>
    /// <returns>Updated pair of stacks after <paramref name="card"/> was ascended.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the given card is not eligible for ascension.</exception>
    /// <seealso cref="CanAscend"/>
    public MajorArcanaStacks Ascend(MajorArcana card) {
        var success = false;
        var newLeft = _left;
        if (card.Value == _left + 1) {
            newLeft = card.Value;
            success = true;
        }

        var newRight = _right;
        if (card.Value == _right - 1) {
            newRight = card.Value;
            success = true;
        }

        if (success) return new MajorArcanaStacks(newLeft, newRight);
        throw new InvalidOperationException("Card value ineligible for ascension");
    }

    /// <summary>
    /// Ascends a streak of cards 
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public MajorArcanaStacks AscendRange(IEnumerable<MajorArcana> cards) => cards.Aggregate(this, (stacks, card) => stacks.Ascend(card));

    public bool CanAscend(MajorArcana card) => _left != _right && (card.Value == _left + 1 || card.Value == _right - 1);

    #region IEquatable

    public bool Equals(MajorArcanaStacks? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _left == other._left && _right == other._right;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MajorArcanaStacks)obj);
    }

    public override int GetHashCode() {
        unchecked {
            return (_left.GetHashCode() * 397) ^ _right.GetHashCode();
        }
    }

    public static bool operator ==(MajorArcanaStacks? left, MajorArcanaStacks? right) => Equals(left, right);

    public static bool operator !=(MajorArcanaStacks? left, MajorArcanaStacks? right) => !Equals(left, right);

    #endregion
}
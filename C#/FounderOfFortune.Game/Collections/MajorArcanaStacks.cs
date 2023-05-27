using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

/// <summary>
/// Represents the pair of major arcana stacks on the board.
/// The stacks are a pair of slots that start empty but can take cards on either end until they meet in the middle.
/// </summary>
/// <remarks>
/// Created as an immutable collection to make backtracking easier.
/// </remarks>
public class MajorArcanaStacks {
    /// <summary>
    /// The lower end of the stack.<br />
    /// When empty, it accepts 0; otherwise, it will only take a card whose value is higher than it by 1.
    /// </summary>
    public readonly MajorArcana? Left;
    /// <summary>
    /// The higher end of the stack.<br />
    /// When empty, it accepts 21; otherwise, it will only take a card whose value is lower than it by 1.
    /// </summary>
    public readonly MajorArcana? Right;

    /// <summary>
    /// Initializes a pair of stacks into empty state.
    /// </summary>
    internal MajorArcanaStacks() {
        Left = null;
        Right = null;
    }

    /// <summary>
    /// Initializes a pair of stacks into the specified state.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="left"/> is greater than <paramref name="right"/>.</exception>
    internal MajorArcanaStacks(MajorArcana? left, MajorArcana? right) {
        if (left > right) throw new ArgumentException("Left cannot be greater than right.");
        Left = left;
        Right = right;
    }

    /// <summary>
    /// Promotes a card to the stacks.
    /// </summary>
    /// <param name="card">The major arcana to promote. Must be eligible for ascension, i.e. greater by 1 than <see cref="Left"/> or lower by 1 than <see cref="Right"/>.</param>
    /// <returns>Updated pair of stacks after <paramref name="card"/> was promoted.</returns>
    /// <exception cref="ArgumentException">Thrown if the given card is not eligible for ascension.</exception>
    /// <seealso cref="CanPromote"/>
    internal MajorArcanaStacks Promote(MajorArcana card) {
        var success = false;
        var newLeft = Left;
        if ((Left == null && card.Value == 0) || (Left != null && card.Value == Left.Value.Value + 1)) {
            newLeft = card;
            success = true;
        }

        var newRight = Right;
        if ((Right == null && card.Value == 21) || (Right != null && card.Value == Right.Value.Value + 1)) {
            newRight = card;
            success = true;
        }

        if (success) return new MajorArcanaStacks(newLeft, newRight);
        throw new ArgumentException("Card value ineligible for ascension", nameof(card));
    }

    /// <summary>
    /// Promotes a streak of cards 
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    internal MajorArcanaStacks PromoteRange(IEnumerable<MajorArcana> cards) => cards.Aggregate(this, (stacks, card) => stacks.Promote(card));

    public bool CanPromote(MajorArcana card) {
        if (Left != null && Right != null && Left == Right) return false;
        return (Left == null && card.Value == 0) || (Left != null && card.Value == Left.Value.Value + 1) ||
               (Right == null && card.Value == 21) || (Right != null && card.Value == Right.Value.Value + 1);
    }
}
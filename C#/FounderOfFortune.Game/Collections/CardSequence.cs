using System.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class CardSequence(Card initialCard, int finalCardValue) : IReadOnlyList<Card>
{
    private int Direction => Math.Sign(finalCardValue - initialCard.Value);
    private bool IsMajorArcana => initialCard.IsMajorArcana;

    #region IReadOnlyList

    public IEnumerator<Card> GetEnumerator()
    {
        var cardValue = initialCard.Value;
        yield return initialCard;
        while (cardValue != finalCardValue)
        {
            cardValue += Direction;
            yield return CreateAtOffset(cardValue - initialCard.Value);
        }
    }

    private Card CreateAtOffset(int offset) => IsMajorArcana ? new Card(initialCard.AsMajorArcana + offset) : new Card(initialCard.AsMinorArcana + offset);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => Math.Abs(initialCard.Value - finalCardValue) + 1;

    public Card this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count, nameof(index));
            return CreateAtOffset(index * Direction);
        }
    }

    #endregion
}
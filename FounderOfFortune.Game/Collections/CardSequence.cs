using System.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class CardSequence : IReadOnlyList<Card> {
    private readonly Card _initialCard;
    private readonly int _finalCardValue;

    public CardSequence(Card initialCard, int finalCardValue) {
        _initialCard = initialCard;
        _finalCardValue = finalCardValue;
    }

    private int Direction => Math.Sign(_finalCardValue - _initialCard.Value);
    private bool IsMajorArcana => _initialCard.IsMajorArcana;

    public IEnumerator<Card> GetEnumerator() {
        var cardValue = _initialCard.Value;
        yield return _initialCard;
        while (cardValue != _finalCardValue) {
            cardValue += Direction;
            yield return CreateAtOffset(cardValue - _initialCard.Value);
        }
    }

    private Card CreateAtOffset(int offset) => IsMajorArcana ? new Card(_initialCard.AsMajorArcana + offset) : new Card(_initialCard.AsMinorArcana + offset);

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public int Count => Math.Abs(_initialCard.Value - _finalCardValue) + 1;

    public Card this[int index] {
        get {
            if (index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            return CreateAtOffset(index * Direction);
        }
    }
}
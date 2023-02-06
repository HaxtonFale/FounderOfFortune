using System.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class CardSequence : IReadOnlyList<Card>, IEquatable<CardSequence> {
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

    #region IReadOnlyList

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

    #endregion

    #region IEquatable

    public bool Equals(CardSequence? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _initialCard.Equals(other._initialCard) && _finalCardValue == other._finalCardValue;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((CardSequence)obj);
    }

    public override int GetHashCode() {
        unchecked {
            return (_initialCard.GetHashCode() * 397) ^ _finalCardValue;
        }
    }

    public static bool operator ==(CardSequence? left, CardSequence? right) {
        return Equals(left, right);
    }

    public static bool operator !=(CardSequence? left, CardSequence? right) {
        return !Equals(left, right);
    }

    #endregion
}
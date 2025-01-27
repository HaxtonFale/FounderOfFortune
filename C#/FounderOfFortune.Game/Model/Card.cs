namespace FounderOfFortune.Game.Model;

public abstract class Card
{
    public abstract int Value { get; }
    public abstract bool IsAdjacentTo(Card other);
    public abstract string FullName { get; }

    public static Card operator +(Card card, int change) => card.ChangeBy(change);

    public static Card operator -(Card card, int change) => card.ChangeBy(-change);

    protected abstract Card ChangeBy(int offset);
}
using System.Collections.Immutable;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game;

public readonly struct BoardState : IEquatable<BoardState>
{
    public readonly MajorArcanaStacks MajorArcanaStacks;
    public readonly MinorArcanaStacks MinorArcanaStacks;
    public readonly Card? FreeCell;
    public readonly ImmutableList<TableauStack> TableauStacks;

    public BoardState(MajorArcanaStacks majorArcanaStacks, MinorArcanaStacks minorArcanaStacks, Card? freeCell,
        ImmutableList<TableauStack> tableauStacks)
    {
        if (tableauStacks.Count != 11)
            throw new ArgumentException("Must have exactly 11 tableau stacks", nameof(tableauStacks));

        var newMajorArcanaStacks = majorArcanaStacks;
        var newMinorArcanaStacks = minorArcanaStacks;
        var newFreeCell = freeCell;
        var newTableauStacks = tableauStacks;

        bool promoted;
        do
        {
            promoted = false;
            if (newFreeCell!= null)
            {
                if (newFreeCell is MajorArcana major && newMajorArcanaStacks.CanPromote(major))
                {
                    newMajorArcanaStacks = newMajorArcanaStacks.Promote(major);
                    newFreeCell = null;
                    promoted = true;
                }
                else if (newFreeCell is MinorArcana minor &&
                         newMinorArcanaStacks.CanPromote(minor))
                {
                    newMinorArcanaStacks = newMinorArcanaStacks.Promote(minor);
                    newFreeCell = null;
                    promoted = true;
                }
            }

            if (promoted) continue;

            for (var i = 0; i < 11; i++)
            {
                var card = newTableauStacks[i].TopCard;
                if (card == null) continue;

                if (card is MajorArcana major && newMajorArcanaStacks.CanPromote(major))
                {
                    newMajorArcanaStacks =
                        newMajorArcanaStacks.PromoteRange(newTableauStacks[i]
                            .TakeCards(out var updatedStack).Cast<MajorArcana>());
                    newTableauStacks = newTableauStacks.SetItem(i, updatedStack);
                    promoted = true;
                    break;
                }

                if (newFreeCell == null && card is MinorArcana minor && newMinorArcanaStacks.CanPromote(minor))
                {
                    newMinorArcanaStacks = newMinorArcanaStacks.PromoteRange(newTableauStacks[i]
                        .TakeCards(out var updatedStack).Cast<MinorArcana>());
                    newTableauStacks = newTableauStacks.SetItem(i, updatedStack);
                    promoted = true;
                    break;
                }
            }
        } while (promoted);

        MajorArcanaStacks = newMajorArcanaStacks;
        MinorArcanaStacks = newMinorArcanaStacks;
        FreeCell = newFreeCell;
        TableauStacks = newTableauStacks;
    }

    public BoardState(ImmutableList<TableauStack> tableauStacks) : this(new MajorArcanaStacks(),
        new MinorArcanaStacks(), null, tableauStacks)
    {
    }

    public BoardState StoreCard(int source)
    {
        if (source is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(source), "Source stack must be between 0 and 10");
        if (FreeCell != null)
            throw new InvalidOperationException("Cannot store a card when free cell already occupied");

        var storedCard = TableauStacks[source].TakeCard(out var updatedTableauStack);
        var updatedTableauStacks = TableauStacks.SetItem(source, updatedTableauStack);

        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, storedCard, updatedTableauStacks);
    }

    public BoardState RetrieveCard(int destination)
    {
        if (destination is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination stack must be between 0 and 10");
        if (FreeCell == null)
            throw new InvalidOperationException("Cannot retrieve a card when free cell is empty");

        var destinationStack = TableauStacks[destination];
        if (destinationStack.TopCard != null && !destinationStack.TopCard.IsAdjacentTo(FreeCell))
            throw new InvalidOperationException($"Cannot place a card on top of stack {destination}");

        var updatedTableau = TableauStacks.SetItem(destination, destinationStack.PlaceCard(FreeCell));
        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, null, updatedTableau);
    }

    public BoardState MoveSingleCard(int source, int destination)
    {
        if (source is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(source), "Source stack must be between 0 and 10");
        if (destination is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination stack must be between 0 and 10");

        var sourceStack = TableauStacks[source];
        if (sourceStack.TopCard == null)
            throw new InvalidOperationException("Cannot move cards from an empty stack");
        var destinationStack = TableauStacks[destination];
        if (destinationStack.TopCard != null && !destinationStack.TopCard.IsAdjacentTo(sourceStack.TopCard))
            throw new InvalidOperationException($"Cannot move cards from stack {source} to stack {destination}");

        var movedCard = sourceStack.TakeCard(out var updatedSourceStack);
        var updatedDestinationStack = destinationStack.PlaceCard(movedCard);
        var updatedTableau = TableauStacks.SetItem(source, updatedSourceStack).SetItem(destination, updatedDestinationStack);

        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, FreeCell, updatedTableau);
    }

    public BoardState MoveCardSequence(int source, int destination)
    {
        if (source is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(source), "Source stack must be between 0 and 10");
        if (destination is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination stack must be between 0 and 10");

        var sourceStack = TableauStacks[source];
        if (sourceStack.TopCard == null)
            throw new InvalidOperationException("Cannot move cards from an empty stack");
        var destinationStack = TableauStacks[destination];
        if (destinationStack.TopCard != null && !destinationStack.TopCard.IsAdjacentTo(sourceStack.TopCard))
            throw new InvalidOperationException($"Cannot move cards from stack {source} to stack {destination}");

        var movedCards = sourceStack.TakeCards(out var updatedSourceStack);
        var updatedDestinationStack = destinationStack.PlaceRange(movedCards);
        var updatedTableau = TableauStacks.SetItem(source, updatedSourceStack).SetItem(destination, updatedDestinationStack);

        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, FreeCell, updatedTableau);
    }

    #region IEquatable

    public bool Equals(BoardState other) =>
        MajorArcanaStacks.Equals(other.MajorArcanaStacks)
        && MinorArcanaStacks.Equals(other.MinorArcanaStacks)
        && FreeCell == other.FreeCell
        && TableauStacks.Equals(other.TableauStacks);

    public override bool Equals(object? obj) => obj is BoardState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(MajorArcanaStacks, MinorArcanaStacks, FreeCell, TableauStacks);

    public static bool operator ==(BoardState left, BoardState right) => left.Equals(right);

    public static bool operator !=(BoardState left, BoardState right) => !left.Equals(right);

    #endregion
}
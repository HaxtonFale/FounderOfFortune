using System.Collections.Immutable;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game;

public readonly struct BoardState {
    public readonly MajorArcanaStacks MajorArcanaStacks;
    public readonly MinorArcanaStacks MinorArcanaStacks;
    public readonly Card? FreeCell;
    public readonly ImmutableList<TableauStack> TableauStacks;

    private BoardState(MajorArcanaStacks majorArcanaStacks, MinorArcanaStacks minorArcanaStacks, Card? freeCell,
        ImmutableList<TableauStack> tableauStacks) {
        if (tableauStacks.Count != 11)
            throw new ArgumentException("Must have exactly 11 tableau stacks.", nameof(tableauStacks));

        var newMajorArcanaStacks = majorArcanaStacks;
        var newMinorArcanaStacks = minorArcanaStacks;
        var newFreeCell = freeCell;
        var newTableauStacks = tableauStacks.ToList();

        var ascended = false;
        do {
            for (var i = 0; i < 11; i++) {
                var card = newTableauStacks[i].TopCard;
                if (card == null) continue;

                var topCard = card.Value;
                if (topCard.IsMajorArcana && newMajorArcanaStacks.CanAscend(topCard.AsMajorArcana)) {
                    newMajorArcanaStacks =
                        newMajorArcanaStacks.AscendRange(newTableauStacks[i]
                            .TakeCards(out var updatedStack).Select(c => c.AsMajorArcana));
                    newTableauStacks[i] = updatedStack;
                    ascended = true;
                }

                if (topCard.IsMinorArcana && newMinorArcanaStacks.CanAscend(topCard.AsMinorArcana) && newFreeCell == null) {
                    newMinorArcanaStacks = newMinorArcanaStacks.AscendRange(newTableauStacks[i]
                        .TakeCards(out var updatedStack).Select(c => c.AsMinorArcana));
                    newTableauStacks[i] = updatedStack;
                    ascended = true;
                }
            }

            if (newFreeCell == null) continue;

            if (newFreeCell.Value.IsMajorArcana && newMajorArcanaStacks.CanAscend(newFreeCell.Value.AsMajorArcana)) {
                newMajorArcanaStacks = newMajorArcanaStacks.Ascend(newFreeCell.Value.AsMajorArcana);
                newFreeCell = null;
                ascended = true;
            }
            else if (newFreeCell.Value.IsMinorArcana &&
                     newMinorArcanaStacks.CanAscend(newFreeCell.Value.AsMinorArcana)) {
                newMinorArcanaStacks = newMinorArcanaStacks.Ascend(newFreeCell.Value.AsMinorArcana);
                newFreeCell = null;
                ascended = true;
            }
        } while (ascended);

        MajorArcanaStacks = newMajorArcanaStacks;
        MinorArcanaStacks = newMinorArcanaStacks;
        FreeCell = newFreeCell;
        TableauStacks = newTableauStacks.ToImmutableList();
    }

    public BoardState(ImmutableList<TableauStack> tableauStacks) : this(new MajorArcanaStacks(),
        new MinorArcanaStacks(), null, tableauStacks) {
    }

    public BoardState StoreCard(int source) {
        if (source is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(source), "Source stack must be between 0 and 10");
        if (FreeCell.HasValue)
            throw new InvalidOperationException("Cannot store a card when free cell already occupied");

        var storedCard = TableauStacks[source].TakeCard(out var updatedTableauStack);
        var updatedTableauStacks = TableauStacks.SetItem(source, updatedTableauStack);

        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, storedCard, updatedTableauStacks);
    }

    public BoardState RetrieveCard(int destination) {
        if (destination is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination stack must be between 0 and 10");
        if (!FreeCell.HasValue)
            throw new InvalidOperationException("Cannot retrieve a card when free cell is empty");

        var destinationStack = TableauStacks[destination];
        if (destinationStack.TopCard != null && !destinationStack.TopCard.Value.IsAdjacentTo(FreeCell.Value))
            throw new InvalidOperationException($"Cannot place a card on top of stack {destination}");

        var updatedTableau = TableauStacks.SetItem(destination, destinationStack.PlaceCard(FreeCell.Value));
        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, null, updatedTableau);
    }

    public BoardState MoveCards(int source, int destination) {
        if (source is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(source), "Source stack must be between 0 and 10");
        if (destination is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination stack must be between 0 and 10");

        var sourceStack = TableauStacks[source];
        if (sourceStack.TopCard == null)
            throw new InvalidOperationException("Cannot move cards from an empty stack");
        var destinationStack = TableauStacks[destination];
        if (destinationStack.TopCard != null && !destinationStack.TopCard.Value.IsAdjacentTo(sourceStack.TopCard.Value))
            throw new InvalidOperationException($"Cannot move cards from stack {source} to stack {destination}");

        var movedCards = sourceStack.TakeCards(out var updatedSourceStack);
        var updatedDestinationStack = destinationStack.PlaceRange(movedCards);
        var updatedTableau = TableauStacks.SetItem(source, updatedSourceStack).SetItem(destination, updatedDestinationStack);

        return new BoardState(MajorArcanaStacks, MinorArcanaStacks, FreeCell, updatedTableau);
    }
}
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
        do
        {
            for (var i = 0; i < 11; i++)
            {
                var card = newTableauStacks[i].TopCard;
                if (card != null)
                {
                    var topCard = card.Value;
                    if (topCard.IsMajorArcana)
                    {
                        var arcana = topCard.AsMajorArcana;
                        var majorStackTops = newMajorArcanaStacks.TopCards;
                        if ((majorStackTops.Item1 == null && arcana.Value == 0)
                            || (majorStackTops.Item1 != null && majorStackTops.Item1.Value + 1 == arcana)
                            || (majorStackTops.Item2 == null && arcana.Value == 21)
                            || (majorStackTops.Item2 != null && majorStackTops.Item2.Value.Value - 1 == arcana.Value))
                        {

                        }
                    }
                }
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
}
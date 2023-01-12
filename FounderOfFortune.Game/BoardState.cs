using System.Collections.Immutable;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using OneOf;

namespace FounderOfFortune.Game;

using Card = OneOf<MajorArcana, MinorArcana>;

public readonly struct BoardState
{
    public readonly MajorArcanaStacks MajorArcanaStacks;
    public readonly MinorArcanaStacks MinorArcanaStacks;
    public readonly Card? FreeCell;
    public readonly ImmutableList<TableauStack> TableauStacks;

    private BoardState(MajorArcanaStacks majorArcanaStacks, MinorArcanaStacks minorArcanaStacks, Card? freeCell,
        ImmutableList<TableauStack> tableauStacks)
    {
        if (tableauStacks.Count != 11)
            throw new ArgumentException("Must have exactly 11 tableau stacks.", nameof(tableauStacks));

        MajorArcanaStacks = majorArcanaStacks;
        MinorArcanaStacks = minorArcanaStacks;
        FreeCell = freeCell;
        TableauStacks = tableauStacks;
    }
}
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game;
using System.Collections.Immutable;

namespace FounderOfFortune.Solver.Test;

public class HeuristicTestsBase
{
    protected static ImmutableList<TableauStack> EmptyStacks =>
        Enumerable.Repeat(new TableauStack(), 11).ToImmutableList();
    protected static BoardState EmptyBoard => new(EmptyStacks);
}
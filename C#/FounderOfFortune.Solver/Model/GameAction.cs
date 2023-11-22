namespace FounderOfFortune.Solver.Model;

public record GameAction;
public record MoveCard(int From, int To) : GameAction;
public record StoreCard(int From) : GameAction;
public record RetrieveCard(int To)  : GameAction;
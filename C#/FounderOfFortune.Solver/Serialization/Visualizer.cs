using FounderOfFortune.Game;
using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.Logging;
using Sharprompt;
using Solver.Core.Serialization;

using BoardSolution = Solver.Core.Solution<FounderOfFortune.Game.BoardState, FounderOfFortune.Solver.Model.GameAction>;

namespace FounderOfFortune.Solver.Serialization;

public class Visualizer(SolutionSerializer<BoardState, GameAction> serializer, ILogger<Visualizer> logger)
{
    private readonly Dictionary<Guid, List<BoardSolution>> _children = new();
    private readonly Dictionary<Guid, BoardSolution> _allSolutions = new();

    public BoardSolution? Root { get; private set; }

    public async Task Load(string file, CancellationToken token = default)
    {
        await using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);

        var deferred = new Queue<(Guid id, Guid parent, BoardState board, GameAction? action)>();

        await foreach (var (id, parent, board, action) in serializer.Deserialize(fs, token))
        {
            logger.LogDebug("Reading deserialized solution: {Id:D}", id);
            if (id == Guid.Empty)
            {
                logger.LogDebug("Found the root solution.");
                var solution = new BoardSolution(board) {Id = id};
                Root = solution;
                _allSolutions[id] = solution;
                _children[id] = new();
            }
            else
            {
                logger.LogDebug("Parent solution: {Id:D}", parent);

                if (!_allSolutions.TryGetValue(parent, out var parentSolution))
                {
                    logger.LogDebug("Parent solution not seen yet. Deferring...");
                    deferred.Enqueue((id, parent, board, action));
                    continue;
                }

                var solution = new BoardSolution(board, (parentSolution, action!)) {Id = id};
                _allSolutions[id] = solution;
                _children[parentSolution.Id].Add(solution);
                _children[id] = new();
            }
        }

        logger.LogDebug("There are {Count} deferred solutions.", deferred.Count);
        while (deferred.Count != 0)
        {
            var entry = deferred.Dequeue();
            logger.LogDebug("Retrying deserialized solution: {Id:D}", entry.id);
            if (!_allSolutions.TryGetValue(entry.parent, out var parent))
            {
                logger.LogDebug("Parent not yet found. Deferring...");
                deferred.Enqueue(entry);
                continue;
            }

            var solution = new BoardSolution(entry.board, (parent, entry.action!)) {Id = entry.id};
            _allSolutions[entry.id] = solution;
            _children[entry.parent].Add(solution);
            _children[entry.id] = new();
        }
    }

    public void InteractiveSession()
    {
        logger.LogInformation("Starting at root solution.");
        var currentItem = Root!;
        var @continue = true;
        while (@continue)
        {
            logger.LogInformation("Current solution: {Id:D} ({Length})", currentItem.Id, currentItem.Length);
            var actions = new List<Action>
            {
                Action.ShowDetail, Action.SelectChild
            };
            if (currentItem != Root)
            {
                actions.Add(Action.SelectSibling);
                actions.Add(Action.SelectPeer);
                actions.Add(Action.SelectParent);
            }
            actions.Add(Action.ListChildren);
            if (currentItem != Root)
            {
                actions.Add(Action.ListSiblings);
                actions.Add(Action.ListPeers);
            }
            actions.Add(Action.ChangeDepth);
            actions.Add(Action.Exit);
            var selection = Prompt.Select("Select action", actions, textSelector: ActionLabel);
            switch (selection)
            {
                case Action.ShowDetail:
                    ShowDetail(currentItem);
                    break;
                case Action.SelectChild:
                    currentItem = SelectChild(currentItem);
                    break;
                case Action.SelectSibling:
                    currentItem = SelectSibling(currentItem);
                    break;
                case Action.SelectPeer:
                    currentItem = SelectPeer(currentItem);
                    break;
                case Action.SelectParent:
                    currentItem = SelectParent(currentItem);
                    break;
                case Action.ChangeDepth:
                    currentItem = ChangeDepth(currentItem);
                    break;
                case Action.ListChildren:
                    ListChildren(currentItem);
                    break;
                case Action.ListSiblings:
                    ListSiblings(currentItem);
                    break;
                case Action.ListPeers:
                    ListPeers(currentItem);
                    break;
                case Action.Exit:
                    @continue = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ShowDetail(BoardSolution solution)
    {
        logger.LogInformation("Solution ID: {Guid:D}{Root}", solution.Id, solution.Id == Guid.Empty ? " (Root)" : string.Empty);
        if (solution.Previous != null)
        {
            var parentId = solution.Previous.Value.Solution.Id;
            logger.LogInformation("Parent ID: {Guid:D}{Root}", parentId, parentId == Guid.Empty ? " (Root)" : string.Empty);
        }
        logger.LogInformation("{Steps} steps from root.", solution.Length);
        logger.LogInformation("Board:\n{Board}", solution.State.RenderBoard());
        logger.LogInformation("Heuristic: {Heuristic}", Heuristics.DepthAndRuns(solution.State));
    }

    private BoardSolution SelectChild(BoardSolution currentItem)
    {
        var children = _children[currentItem.Id];
        return SelectSolution(currentItem, children, "child");
    }

    private BoardSolution SelectSibling(BoardSolution currentItem)
    {
        if (currentItem.Previous == null)
        {
            logger.LogError("Cannot select siblings of the root solution!");
            return currentItem;
        }

        return SelectSolution(currentItem, GetSiblings(currentItem), "sibling");
    }

    private BoardSolution SelectPeer(BoardSolution currentItem)
    {
        if (currentItem.Previous == null)
        {
            logger.LogError("Cannot select peers of the root solution!");
            return currentItem;
        }

        return SelectSolution(currentItem, GetPeers(currentItem), "peer");
    }

    private BoardSolution SelectSolution(BoardSolution currentItem, IReadOnlyCollection<BoardSolution> selection, string noun)
    {
        if (selection.Count == 0)
        {
            logger.LogInformation("No {Noun} solutions available.", noun);
            return currentItem;
        }

        try
        {
            return Prompt.Select($"Select {noun}", selection, textSelector: SolutionLabel);
        }
        catch (Exception)
        {
            return currentItem;
        }
    }

    private BoardSolution SelectParent(BoardSolution currentItem)
    {
        if (currentItem.Previous == null)
        {
            logger.LogError("Cannot select a parent of the root solution!");
            return currentItem;
        }

        var parentSolution = currentItem.Previous.Value.Solution;
        logger.LogInformation("Selecting parent solution ({Guid:D})", parentSolution.Id);
        return parentSolution;
    }

    private BoardSolution ChangeDepth(BoardSolution currentItem)
    {
        var currentDepth = currentItem.Length;
        BoardSolution? selected = null;
        while (selected == null)
        {
            var newDepth = Prompt.Input<uint>("Select new depth", currentDepth);

            try
            {
                var options = _allSolutions.Values.Where(s => s.Length == newDepth).ToList();
                if (options.Count == 1)
                {
                    selected = options[0];
                    logger.LogInformation("Selecting solution {Guid:D}", selected.Id);
                }
                else
                {
                    selected = Prompt.Select($"Select solution at depth {newDepth}", options,
                        textSelector: SolutionLabel);
                }
            }
            catch (Exception)
            {
                selected = currentItem;
            }
        }

        return selected;
    }

    private void ListSiblings(BoardSolution currentItem)
    {
        if (currentItem.Previous == null)
        {
            logger.LogError("Cannot list siblings of the root solution!");
            return;
        }

        ListSolutions(GetSiblings(currentItem));
    }

    private void ListPeers(BoardSolution currentItem)
    {
        if (currentItem.Previous == null)
        {
            logger.LogError("Cannot list peers of the root solution!");
            return;
        }

        ListSolutions(GetPeers(currentItem));
    }

    private void ListChildren(BoardSolution currentItem)
    {
        ListSolutions(_children[currentItem.Id]);
    }

    private void ListSolutions(List<BoardSolution> solutions)
    {
        foreach (var solution in solutions)
        {
            logger.LogInformation("{Id:D}\n{Board}", solution.Id, solution.State.RenderBoard());
        }
    }

    private List<BoardSolution> GetSiblings(BoardSolution solution) => _children[solution.Previous!.Value.Solution.Id];

    private List<BoardSolution> GetPeers(BoardSolution solution) => _allSolutions.Values.Where(s => s.Length == solution.Length).ToList();

    private enum Action
    {
        Exit,
        SelectChild,
        SelectSibling,
        SelectPeer,
        SelectParent,
        ListChildren,
        ListSiblings,
        ListPeers,
        ChangeDepth,
        ShowDetail
    }

    private static string ActionLabel(Action action) => action switch
    {
        Action.Exit => "Exit",
        Action.SelectChild => "Select a child solution",
        Action.SelectSibling => "Select a sibling solution",
        Action.SelectPeer => "Select a solution on the same level",
        Action.SelectParent => "Select a parent solution",
        Action.ListChildren => "List child solutions",
        Action.ListSiblings => "List sibling solutions",
        Action.ListPeers => "List solutions on the same level",
        Action.ChangeDepth => "Change search depth",
        Action.ShowDetail => "Show solution details",
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, "No label defined for action")
    };

    private static string SolutionLabel(BoardSolution solution)
    {
        var output = solution.Previous!.Value.Step switch
        {
            MoveCard m => $"Move cards from {m.From} to {m.To}",
            StoreCard s => $"Store card from {s.From}",
            RetrieveCard r => $"Retrieve card to {r.To}",
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"{output} ({solution.Id:D})";
    }
}
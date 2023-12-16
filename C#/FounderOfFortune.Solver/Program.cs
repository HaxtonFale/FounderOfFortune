using System.Collections.Immutable;
using Cocona;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver;
using FounderOfFortune.Solver.Model;
using FounderOfFortune.Solver.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Solver.Core;
using Solver.Core.Base;
using Solver.Core.Serialization;

const string dumpFilePath = @"D:\FF solving\dump.bin";

var builder = CoconaApp.CreateBuilder(args);
var logLevelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((_, _, config) =>
{
    config.MinimumLevel.ControlledBy(logLevelSwitch)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services
    .AddSingleton(provider =>
    {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger("Founder");
    }).AddSingleton<Solver<BoardState, GameAction>, AStarSolver<BoardState, GameAction, int>>()
    .AddSingleton(Heuristics.DepthAndRuns);

var app = builder.Build();
app.AddCommand(async (CoconaAppContext ctx, Microsoft.Extensions.Logging.ILogger logger, bool verbose, bool moveSingleCards) =>
{
    const string cards = """
 C7,19,S6,S11,C6,17,4
 S13,S5,10,W4,9,W12,18
 W8,7,C11,C2,S9,3,W10
 S7,G13,G11,13,11,G3,G12
 G8,16,1,8,G9,C9,S8

 G10,G7,G5,W5,6,W9,C5
 S4,W11,14,20,W3,12,C13
 21,S10,0,W2,C4,W7,C10
 C3,2,S2,G2,W13,S12,C12
 S3,15,C8,G4,G6,W6,5
 """;
    if (verbose) logLevelSwitch.MinimumLevel = LogEventLevel.Verbose;

    var solver = new AStarSolver<BoardState, GameAction, int>(board => board.GetValidActions(),
        (board, action) => board.PerformAction(action, moveSingleCards), board => board.IsComplete(),
        Heuristics.DepthAndRuns);

    var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

    var board = GenerateBoardState(cards);
    var solution = solver.TrySolve(board, tokenSource.Token);
    logger.LogInformation(solution == null ? "No solution found." : "Solution found.");
    await using var stream = new FileStream(dumpFilePath, FileMode.Create);
    var serializer = new SolutionSerializer<BoardState, GameAction>(new BoardSerializer());
    await serializer.Serialize(solver, stream);
});

await app.RunAsync();
return;

static BoardState GenerateBoardState(string cardString)
{
    var stacks = cardString.Split('\n').Select(line =>
    {
        if (string.IsNullOrWhiteSpace(line)) return new TableauStack();
        var cards = line.Split(',').Select<string, Card>(card =>
        {
            if (char.IsDigit(card[0]))
            {
                var value = int.Parse(card);
                return new MajorArcana(value);
            }
            else
            {
                var suit = card[0] switch
                {
                    'C' => Suit.Coins,
                    'S' => Suit.Swords,
                    'W' => Suit.Wands,
                    'G' => Suit.Goblets,
                    _ => throw new ArgumentOutOfRangeException($"Invalid character: {card[0]}")
                };
                var value = int.Parse(card[1..]);
                return new MinorArcana(suit, value);
            }
        });
        return new TableauStack(cards.ToImmutableList());
    }).ToImmutableList();
    return new BoardState(stacks);
}
using System.Collections.Immutable;
using Cocona;
using FounderOfFortune.Game;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;
using FounderOfFortune.Solver.Parallel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

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
});

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

    var board = GenerateBoardState(cards);
    var solver = new TaskSolver();
    var solution = await solver.TrySolve(board, moveSingleCards, ctx.CancellationToken);
    if (solution == null)
    {
        logger.LogInformation("No solution found.");
    }
    else
    {
        logger.LogInformation("Solution found:");
        var step = 1;
        foreach (var line in solution.RenderSolution())
        {
            logger.LogInformation("{Step}. {Line}", step++, line);
        }
    }
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
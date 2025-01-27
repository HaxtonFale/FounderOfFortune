using Cocona;
using FounderOfFortune.Game;
using FounderOfFortune.Solver.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Solver.Core.Serialization;
using MSLogger = Microsoft.Extensions.Logging.ILogger;

const string dumpFilePath = @"D:\FF solving\dump.bin";

var builder = CoconaApp.CreateBuilder(args);
var logLevelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((_, _, config) => {
    config.MinimumLevel.ControlledBy(logLevelSwitch)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services
    .AddSingleton(provider => {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger("Founder");
    })
    .AddSingleton<IStateSerializer<BoardState, GameAction>, BoardSerializer>()
    .AddSingleton<SolutionSerializer<BoardState, GameAction>>()
    .AddSingleton(Heuristics.DepthAndRuns);
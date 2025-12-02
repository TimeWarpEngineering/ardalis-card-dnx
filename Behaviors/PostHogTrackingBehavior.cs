using System.Threading;
using System.Threading.Tasks;
using Ardalis.Cli.Telemetry;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;

namespace Ardalis.Cli.Behaviors;

/// <summary>
/// Pipeline behavior that automatically tracks ALL command execution with PostHog.
/// Works for both delegate routes and Mediator commands - single unified pipeline.
/// </summary>
public sealed class PostHogTrackingBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly PostHogService _postHog;
    private readonly RouteExecutionContext _executionContext;
    private readonly ILogger<PostHogTrackingBehavior<TMessage, TResponse>> _logger;

    public PostHogTrackingBehavior(
        PostHogService postHog,
        RouteExecutionContext executionContext,
        ILogger<PostHogTrackingBehavior<TMessage, TResponse>> logger)
    {
        _postHog = postHog;
        _executionContext = executionContext;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        // Extract command name from route pattern
        // Examples: "blog" -> "blog", "packages --all?" -> "packages", "dotnetconf-score {year:int?}" -> "dotnetconf-score"
        string commandName = _executionContext.RoutePattern
            .Split(' ')[0]
            .ToLowerInvariant();

        _logger.LogDebug("Tracking command: {CommandName}", commandName);
        _postHog.TrackCommand(commandName);

        return await next(message, cancellationToken);
    }
}

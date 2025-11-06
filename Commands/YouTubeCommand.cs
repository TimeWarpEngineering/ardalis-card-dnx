using Spectre.Console.Cli;

namespace Ardalis.Commands;

public class YouTubeCommand : Command
{
    public override int Execute(CommandContext context)
    {
        CommandHandlers.OpenYouTube();
        return 0;
    }
}

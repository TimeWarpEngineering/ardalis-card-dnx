using Spectre.Console.Cli;

namespace Ardalis.Commands;

public class CardCommand : Command
{
    public override int Execute(CommandContext context)
    {
        CommandHandlers.ShowCard();
        return 0;
    }
}

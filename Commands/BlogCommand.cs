using Spectre.Console.Cli;

namespace Ardalis.Commands;

public class BlogCommand : Command
{
    public override int Execute(CommandContext context)
    {
        CommandHandlers.OpenBlog();
        return 0;
    }
}

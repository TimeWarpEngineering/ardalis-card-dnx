using Ardalis.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Threading;

namespace Ardalis.Commands;

public class LinkedInCommand : Command
{
    public override int Execute(CommandContext context, CancellationToken cancellationToken = default)
    {
        var url = "https://www.linkedin.com/in/stevenandrewsmith/";
        AnsiConsole.MarkupLine($"[bold blue]Opening LinkedIn profile:[/] {url}");
        UrlHelper.Open(url);
        return 0;
    }
}

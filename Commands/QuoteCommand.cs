using Spectre.Console.Cli;
using System.Threading.Tasks;

namespace Ardalis.Commands;

public class QuoteCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        await CommandHandlers.ShowQuoteAsync();
        return 0;
    }
}

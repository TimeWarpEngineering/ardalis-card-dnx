using System.Threading.Tasks;
using Ardalis.Helpers;
using TimeWarp.Nuru;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays a random Ardalis quote using Nuru terminal.
/// </summary>
public static class QuoteHandler
{
    public static async Task ExecuteAsync()
    {
        ITerminal terminal = NuruTerminal.Default;

        string quote = await QuoteHelper.GetRandomQuote();

        terminal.WriteLine();
        terminal.WritePanel(panel => panel
            .Content($"\"{quote}\"".Italic() + "\n\n" + "— Ardalis".Gray())
            .Border(BorderStyle.Rounded)
            .BorderColor(AnsiColors.Cyan)
            .Padding(2, 1));
    }
}

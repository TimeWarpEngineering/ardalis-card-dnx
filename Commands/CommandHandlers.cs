using Spectre.Console;
using System.Threading.Tasks;

namespace Ardalis.Commands;

public static class CommandHandlers
{
    public static void ShowCard()
    {
        // Top rule with standard color
        var top = new Rule("[deepskyblue3]────────────────────────────────────────[/]")
        {
            Justification = Justify.Center
        };
        AnsiConsole.Write(top);

        // Card content
        var panelContent = new Markup(
            "[bold mediumorchid1]Steve 'Ardalis' Smith[/]\n" +
            "[grey]Software Architect & Trainer[/]\n\n" +
            "[link=https://ardalis.com][deepskyblue3]https://ardalis.com[/][/]\n" +
            "[link=https://nimblepros.com][violet]https://nimblepros.com[/][/]\n\n" +
            "[italic grey]Clean Architecture • DDD • .NET[/]"
        );

        // Panel with purple border, not full-width
        var panel = new Panel(panelContent)
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.MediumOrchid1),
            Padding = new Padding(2, 1, 2, 1),
            Expand = false
        };

        // Simple header (no alignment property on some Spectre versions)
        panel.Header = new PanelHeader("[bold deepskyblue3]💠 Ardalis[/]");

        // Center the whole panel (Spectre.Console centers non-expanded panels by default)
        AnsiConsole.Write(panel);

        // Bottom rule with standard color
        var bottom = new Rule("[mediumorchid1]────────────────────────────────────────[/]")
        {
            Justification = Justify.Center
        };
        AnsiConsole.Write(bottom);

        AnsiConsole.MarkupLine("\n[dim]Try '[deepskyblue3]ardalis blog[/]' or '[mediumorchid1]ardalis youtube[/]' for more options[/]");
    }

    public static void OpenBlog()
    {
        var url = "https://ardalis.com";
        AnsiConsole.MarkupLine($"[bold green]Opening blog:[/] {url}");
        Helpers.UrlHelper.Open(url);
    }

    public static void OpenYouTube()
    {
        var url = "https://youtube.com/@Ardalis";
        AnsiConsole.MarkupLine($"[bold red]Opening YouTube channel:[/] {url}");
        Helpers.UrlHelper.Open(url);
    }

    public static async Task ShowQuoteAsync()
    {
        var quote = await Helpers.QuoteHelper.GetRandomQuote();
        AnsiConsole.WriteLine($"\"{quote}\" - Ardalis");
    }
}

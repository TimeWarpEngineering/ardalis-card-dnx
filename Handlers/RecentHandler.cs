#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeWarp.Nuru;
using static Ardalis.Helpers.RecentHelper;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays recent activity from various sources using Nuru table widget.
/// </summary>
/// <remarks>
/// Note: The verbose mode in RecentHelper still uses Spectre.Console for progress output.
/// This will be addressed when RecentHelper is migrated in task 015.
/// </remarks>
public static class RecentHandler
{
    public static async Task ExecuteAsync(bool verbose)
    {
        ITerminal terminal = NuruTerminal.Default;

        terminal.WriteLine("Fetching recent activity...".Bold());
        terminal.WriteLine();

        // GetRecentActivitiesAsync handles verbose output internally (via Spectre for now)
        List<RecentActivity> activities = await GetRecentActivitiesAsync(verbose);

        if (activities.Count == 0)
        {
            terminal.WriteLine("No recent activities found.".Yellow());
            return;
        }

        Table table = new Table()
            .AddColumn("Source", Alignment.Center)
            .AddColumn("Activity", Alignment.Left)
            .AddColumn("When", Alignment.Right)
            .AddColumn("Link", Alignment.Center);

        table.Border = BorderStyle.Rounded;

        foreach (RecentActivity activity in activities)
        {
            string truncatedTitle = activity.GetTruncatedTitle(60);
            string sourceWithIcon = $"{activity.Icon} {activity.Source}";
            string when = activity.GetRelativeTimeString();

            // Add UTM tracking to URL
            string urlWithTracking = AddUtmSource(activity.Url);
            string link = "Click for details".Link(urlWithTracking).Cyan();

            table.AddRow(sourceWithIcon, truncatedTitle, when, link);
        }

        terminal.WriteTable(table);
    }
}

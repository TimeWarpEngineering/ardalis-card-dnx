#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Api;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays top videos from .NET Conf playlists using Nuru table widget.
/// </summary>
public sealed class DotNetConfScoreCommand : IRequest
{
    public int? Year { get; init; }

    public sealed class Handler : IRequestHandler<DotNetConfScoreCommand>
    {
        private const string PlaylistsJsonUrl = "https://ardalis.com/playlists.json";
        private const string EncodedPayload = "c2d3cTRzQlJMVUxyWThXOXcyeUdEcFY5aGRQSGNTS3ZHNHVwdjcwcmFDQQ==";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IHttpClientFactory httpClientFactory,
            ILogger<Handler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async ValueTask<Unit> Handle(
            DotNetConfScoreCommand request,
            CancellationToken cancellationToken)
        {
            int year = request.Year ?? DateTime.Now.Year;
            ITerminal terminal = NuruTerminal.Default;

            terminal.WriteLine($".NET Conf {year} - Top Videos by Views".Green().Bold());
            terminal.WriteLine();

            try
            {
                // Fetch playlists data using ArdalisWeb client
                List<PlaylistInfo> playlists = await FetchPlaylistsDataAsync(cancellationToken);

                // Find matching playlist
                PlaylistInfo? playlist = playlists.FirstOrDefault(p =>
                    p.Name.Contains(year.ToString(), StringComparison.OrdinalIgnoreCase));

                if (playlist == null)
                {
                    terminal.WriteLine($"No playlist found for .NET Conf {year}".Yellow());
                    IEnumerable<int> availableYears = playlists
                        .Select(p => ExtractYear(p.Name))
                        .Where(y => y.HasValue)
                        .Select(y => y!.Value);
                    terminal.WriteLine($"Available years: {string.Join(", ", availableYears)}".Gray());
                    return Unit.Value;
                }

                // Use ArdalisApi client for video stats
                HttpClient apiClient = _httpClientFactory.CreateClient("ArdalisApi");
                string apiKey = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(EncodedPayload));
                var client = new ArdalisApiClient(apiClient, apiKey);

                List<VideoDetails> topVideos = await client.GetTopVideosAsync(playlist.Url);

                // Get highlight video IDs
                HashSet<string> highlightVideoIds = playlist.HighlightVideos
                    .Select(v => ExtractVideoId(v.Url))
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToHashSet()!;

                if (topVideos.Count == 0)
                {
                    terminal.WriteLine("No videos found in the playlist".Yellow());
                    return Unit.Value;
                }

                // Display in a table
                DisplayVideosTable(terminal, topVideos, highlightVideoIds);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching .NET Conf data for year {Year}", year);
                terminal.WriteLine($"Error fetching data: {ex.Message}".Red());
                terminal.WriteLine();

                if (ex.Message.Contains("403"))
                {
                    terminal.WriteLine("YouTube API returned 403 Forbidden. Please check:".Yellow());
                    terminal.WriteLine("  1. Your API key is valid");
                    terminal.WriteLine("  2. YouTube Data API v3 is enabled in your Google Cloud project");
                    terminal.WriteLine("     https://console.cloud.google.com/apis/library/youtube.googleapis.com");
                    terminal.WriteLine("  3. Your API key has the proper restrictions (or none for testing)");
                    terminal.WriteLine("  4. You haven't exceeded your daily quota");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching .NET Conf data for year {Year}", year);
                terminal.WriteLine($"Unexpected error: {ex.Message}".Red());
            }

            return Unit.Value;
        }

        private async Task<List<PlaylistInfo>> FetchPlaylistsDataAsync(CancellationToken cancellationToken)
        {
            HttpClient webClient = _httpClientFactory.CreateClient("ArdalisWeb");
            string response = await webClient.GetStringAsync(PlaylistsJsonUrl, cancellationToken);
            List<PlaylistInfo>? playlists = JsonSerializer.Deserialize<List<PlaylistInfo>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return playlists ?? [];
        }

        private static void DisplayVideosTable(ITerminal terminal, List<VideoDetails> videos, HashSet<string> highlightVideoIds)
        {
            Table table = new Table()
                .AddColumn("Rank", Alignment.Center)
                .AddColumn("Title", Alignment.Left)
                .AddColumn("Views", Alignment.Right);

            table.Border = BorderStyle.Rounded;

            int rank = 1;
            foreach (VideoDetails video in videos)
            {
                bool isHighlighted = highlightVideoIds.Contains(video.Id);
                string title = video.Title.Length > 80 ? video.Title[..77] + "..." : video.Title;
                string views = video.ViewCount.ToString("N0");

                if (isHighlighted)
                {
                    table.AddRow(
                        rank.ToString().Yellow().Bold(),
                        ("⭐ " + title).Link(video.Url).Yellow().Bold(),
                        views.Yellow().Bold()
                    );
                }
                else
                {
                    table.AddRow(
                        rank.ToString().Gray(),
                        title.Link(video.Url),
                        views.Gray()
                    );
                }

                rank++;
            }

            terminal.WriteTable(table);
            terminal.WriteLine();
            terminal.WriteLine("⭐ indicates Ardalis's video".Gray());
        }

        private static string? ExtractVideoId(string url)
        {
            Match match = Regex.Match(url, @"[?&]v=([^&]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static int? ExtractYear(string name)
        {
            Match match = Regex.Match(name, @"\b(20\d{2})\b");
            return match.Success && int.TryParse(match.Groups[1].Value, out int year) ? year : null;
        }
    }

    // JSON models for playlists.json
    private sealed class PlaylistInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("highlight-videos")]
        public List<HighlightVideo> HighlightVideos { get; set; } = [];
    }

    private sealed class HighlightVideo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}

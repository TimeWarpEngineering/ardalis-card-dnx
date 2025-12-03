#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;
using static Ardalis.Cli.Urls;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays popular Ardalis GitHub repositories using Nuru table widget.
/// </summary>
public sealed class ReposCommand : IRequest
{
    public sealed class Handler : IRequestHandler<ReposCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Handler> _logger;

        private static readonly string[] RepoNames =
        [
            "CleanArchitecture",
            "Specification",
            "GuardClauses",
            "Result",
            "SmartEnum"
        ];

        public Handler(
            IHttpClientFactory httpClientFactory,
            ILogger<Handler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async ValueTask<Unit> Handle(
            ReposCommand request,
            CancellationToken cancellationToken)
        {
            ITerminal terminal = NuruTerminal.Default;

            terminal.WriteLine("Ardalis's Popular GitHub Repositories".Green().Bold());
            terminal.WriteLine();

            HttpClient client = _httpClientFactory.CreateClient("GitHub");
            List<(string Name, GitHubRepo Info)> repos = [];

            foreach (string repoName in RepoNames)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(
                        $"repos/ardalis/{repoName}", cancellationToken);
                    response.EnsureSuccessStatusCode();

                    GitHubRepo? repoInfo = await response.Content
                        .ReadFromJsonAsync<GitHubRepo>(cancellationToken);

                    if (repoInfo != null)
                        repos.Add((repoName, repoInfo));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch repo {RepoName}", repoName);
                }
            }

            repos.Sort((a, b) =>
                b.Info.StargazersCount.CompareTo(a.Info.StargazersCount));

            DisplayTable(terminal, repos);

            return Unit.Value;
        }

        private static void DisplayTable(ITerminal terminal, List<(string Name, GitHubRepo Info)> repos)
        {
            Table table = new Table()
                .AddColumn("Repository")
                .AddColumn("Stars", Alignment.Right)
                .AddColumn("Description");

            table.Border = BorderStyle.Rounded;

            foreach ((string repoName, GitHubRepo repoInfo) in repos)
            {
                string description = repoInfo.Description ?? "No description";
                if (description.Length > 60)
                    description = description[..57] + "...";

                table.AddRow(
                    repoName.Link(repoInfo.HtmlUrl).Cyan(),
                    $"\u2b50 {repoInfo.StargazersCount:N0}".Yellow(),
                    description.Gray()
                );
            }

            terminal.WriteTable(table);
            terminal.WriteLine();
            terminal.WriteLine("Visit: ".Gray() + GitHub.Link(GitHub).Cyan());
        }
    }

    private sealed class GitHubRepo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("stargazers_count")]
        public int StargazersCount { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
    }
}

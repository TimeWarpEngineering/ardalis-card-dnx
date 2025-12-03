#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;
using static Ardalis.Cli.Urls;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays popular Ardalis NuGet packages using Nuru table widget.
/// </summary>
public sealed class PackagesCommand : IRequest
{
    public bool All { get; init; }
    public int? Size { get; init; }

    public sealed class Handler : IRequestHandler<PackagesCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Handler> _logger;

        private static readonly PackageInfo[] FallbackPackages =
        [
            new("Ardalis.GuardClauses", "Guard clause extensions for validating method arguments", 36255041),
            new("Ardalis.Specification", "Base classes for implementing the Specification pattern", 13083928),
            new("Ardalis.Result", "A result abstraction for modeling success/failure without exceptions", 6326436),
            new("Ardalis.ApiEndpoints", "A library for organizing API endpoints using MediatR-like handlers", 4437628),
            new("Ardalis.SmartEnum", "A type-safe, extensible alternative to enums", 19914393),
            new("Ardalis.HttpClientTestExtensions", "Test helpers for testing code that uses HttpClient", 1062914),
            new("Ardalis.SharedKernel", "Base types for Domain-Driven Design and Clean Architecture", 414411)
        ];

        public Handler(
            IHttpClientFactory httpClientFactory,
            ILogger<Handler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async ValueTask<Unit> Handle(
            PackagesCommand request,
            CancellationToken cancellationToken)
        {
            ITerminal terminal = NuruTerminal.Default;

            terminal.WriteLine("Ardalis's Popular NuGet Packages".Green().Bold());
            terminal.WriteLine();

            // Try to fetch packages from NuGet API, fall back to hardcoded list if it fails
            PackageInfo[]? packages = await GetPackagesFromApiAsync(request.All, cancellationToken);
            packages ??= FallbackPackages;

            int pageSize = request.Size ?? 10;

            if (request.All)
            {
                DisplayPackagesTable(terminal, packages);
            }
            else
            {
                // Display packages with paging
                List<PackageInfo> packagesList = [.. packages];
                int currentIndex = 0;

                while (currentIndex < packagesList.Count)
                {
                    int endIndex = Math.Min(currentIndex + pageSize, packagesList.Count);
                    PackageInfo[] pagePackages = packagesList.Skip(currentIndex).Take(endIndex - currentIndex).ToArray();

                    DisplayPackagesTable(terminal, pagePackages);

                    currentIndex = endIndex;

                    if (currentIndex < packagesList.Count)
                    {
                        terminal.WriteLine();
                        terminal.Write("Press ".Gray());
                        terminal.Write("Space".Bold());
                        terminal.Write(" for more, or any other key to exit...".Gray());

                        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                        terminal.WriteLine();

                        if (key.Key != ConsoleKey.Spacebar)
                        {
                            terminal.WriteLine($"Showing {currentIndex} of {packagesList.Count} packages".Gray());
                            break;
                        }
                        terminal.WriteLine();
                    }
                }
            }

            terminal.WriteLine();
            terminal.WriteLine("Visit: ".Gray() + NuGet.Link(NuGet).Cyan());

            return Unit.Value;
        }

        private static void DisplayPackagesTable(ITerminal terminal, PackageInfo[] packages)
        {
            Table table = new Table()
                .AddColumn("Package")
                .AddColumn("Downloads", Alignment.Right)
                .AddColumn("Description");

            table.Border = BorderStyle.Rounded;

            foreach (PackageInfo package in packages)
            {
                string downloads = $"📦 {package.TotalDownloads:N0}";
                string description = package.Description;

                if (description.Length > 50)
                {
                    description = description[..47] + "...";
                }

                string nugetUrl = $"https://www.nuget.org/packages/{package.Name}";
                string urlWithTracking = AddUtmSource(nugetUrl);

                table.AddRow(
                    package.Name.Link(urlWithTracking).Cyan(),
                    downloads.Yellow(),
                    description.Gray()
                );
            }

            terminal.WriteTable(table);
        }

        private async Task<PackageInfo[]?> GetPackagesFromApiAsync(bool showAll, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("NuGet");
                NuGetSearchResult? searchResult = await client.GetFromJsonAsync<NuGetSearchResult>(
                    "query?q=owner:ardalis&take=100",
                    cancellationToken);

                if (searchResult?.Data == null)
                {
                    return null;
                }

                IEnumerable<NuGetPackageData> packages = searchResult.Data;

                // Filter packages: only include those with 0 or 1 periods (exclude sub-packages)
                if (!showAll)
                {
                    packages = packages.Where(p => p.Id.Count(c => c == '.') <= 1);
                }

                return packages
                    .OrderByDescending(p => p.TotalDownloads)
                    .Select(p => new PackageInfo(p.Id, p.Description ?? "", p.TotalDownloads))
                    .ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch packages from NuGet API");
                return null;
            }
        }
    }

    private sealed record PackageInfo(string Name, string Description, long TotalDownloads = 0);

    private sealed class NuGetSearchResult
    {
        public NuGetPackageData[]? Data { get; set; }
    }

    private sealed class NuGetPackageData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("totalDownloads")]
        public long TotalDownloads { get; set; }
    }
}

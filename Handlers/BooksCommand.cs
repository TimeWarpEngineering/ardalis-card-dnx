#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;
using static Ardalis.Cli.Urls;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays published books using Nuru panel widget with paging support.
/// </summary>
public sealed class BooksCommand : IRequest
{
    public bool NoPaging { get; init; }
    public int? Size { get; init; }

    public sealed class Handler : IRequestHandler<BooksCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Handler> _logger;

        private static readonly BookInfo[] FallbackBooks =
        [
            new BookInfo
            {
                Title = "Architecting Modern Web Applications with ASP.NET Core and Microsoft Azure",
                Link = "https://ardalis.com/architecture-ebook/",
                CoverImage = "https://ardalis.com/img/Architecture-eBook-Cover-242x300.png",
                Description = "Learn how to architect modern web applications using ASP.NET Core and Azure. This free eBook covers architecture patterns, clean code practices, and cloud deployment strategies.",
                Publisher = "Microsoft",
                PublicationDate = "2023"
            }
        ];

        public Handler(
            IHttpClientFactory httpClientFactory,
            ILogger<Handler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async ValueTask<Unit> Handle(
            BooksCommand request,
            CancellationToken cancellationToken)
        {
            ITerminal terminal = NuruTerminal.Default;

            terminal.WriteLine("Ardalis's Published Books".Blue().Bold());
            terminal.WriteLine();

            List<BookInfo> books;

            try
            {
                books = await FetchBooksFromUrlAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch books from API, using fallback");
                terminal.WriteLine("Using fallback book list...".Gray());
                terminal.WriteLine();
                books = [.. FallbackBooks];
            }

            if (books.Count == 0)
            {
                terminal.WriteLine("No books available at the moment.".Yellow());
                return Unit.Value;
            }

            // Sort books by publication date (most recent first)
            List<BookInfo> sortedBooks = books
                .OrderByDescending(b => ParsePublicationYear(b.PublicationDate))
                .ToList();

            int pageSize = request.Size ?? 10;

            if (request.NoPaging)
            {
                // Display all books without paging
                foreach (BookInfo book in sortedBooks)
                {
                    DisplayBook(terminal, book);
                }
            }
            else
            {
                // Display books with paging
                int currentIndex = 0;

                while (currentIndex < sortedBooks.Count)
                {
                    int endIndex = Math.Min(currentIndex + pageSize, sortedBooks.Count);
                    List<BookInfo> pageBooks = sortedBooks.Skip(currentIndex).Take(endIndex - currentIndex).ToList();

                    foreach (BookInfo book in pageBooks)
                    {
                        DisplayBook(terminal, book);
                    }

                    currentIndex = endIndex;

                    if (currentIndex < sortedBooks.Count)
                    {
                        terminal.Write("Press ".Gray());
                        terminal.Write("Space".Bold());
                        terminal.Write(" for more, or any other key to exit...".Gray());

                        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                        terminal.WriteLine();

                        if (key.Key != ConsoleKey.Spacebar)
                        {
                            terminal.WriteLine($"Showing {currentIndex} of {sortedBooks.Count} books".Gray());
                            break;
                        }
                        terminal.WriteLine();
                    }
                }
            }

            terminal.WriteLine();
            terminal.WriteLine("Learn more at: ".Gray() + Books.Link(Books).Cyan());

            return Unit.Value;
        }

        private static void DisplayBook(ITerminal terminal, BookInfo book)
        {
            string urlWithTracking = AddUtmSource(book.Link);
            string displayUrl = StripQueryString(book.Link);

            string description = string.IsNullOrEmpty(book.Description)
                ? "No description available".Gray()
                : book.Description;

            string publisher = string.IsNullOrEmpty(book.Publisher)
                ? "N/A"
                : book.Publisher;

            string publicationDate = string.IsNullOrEmpty(book.PublicationDate)
                ? "N/A"
                : book.PublicationDate;

            string content =
                book.Title.Bold() + "\n\n" +
                description + "\n\n" +
                "Publisher: ".Gray() + publisher + "\n" +
                "Published: ".Gray() + publicationDate + "\n\n" +
                "Learn more: ".Gray() + displayUrl.Link(urlWithTracking).Cyan();

            terminal.WritePanel(panel => panel
                .Content(content)
                .Border(BorderStyle.Rounded)
                .BorderColor(AnsiColors.Blue)
                .Padding(1, 0));

            terminal.WriteLine();
        }

        private async Task<List<BookInfo>> FetchBooksFromUrlAsync(CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("ArdalisWeb");
            List<BookInfo>? books = await client.GetFromJsonAsync<List<BookInfo>>(
                "https://ardalis.com/books.json",
                cancellationToken);
            return books ?? [];
        }

        private static int ParsePublicationYear(string publicationDate)
        {
            if (string.IsNullOrWhiteSpace(publicationDate))
                return 0;

            // Try to parse as a year directly
            if (int.TryParse(publicationDate, out int year))
                return year;

            // Try to extract year from various date formats
            if (DateTime.TryParse(publicationDate, out DateTime date))
                return date.Year;

            // If all else fails, try to find a 4-digit year in the string
            Match match = Regex.Match(publicationDate, @"\b(19|20)\d{2}\b");
            if (match.Success && int.TryParse(match.Value, out int extractedYear))
                return extractedYear;

            return 0;
        }
    }

    private sealed class BookInfo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;

        [JsonPropertyName("coverImage")]
        public string CoverImage { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; } = string.Empty;

        [JsonPropertyName("publicationDate")]
        public string PublicationDate { get; set; } = string.Empty;

        [JsonPropertyName("isbn")]
        public string Isbn { get; set; } = string.Empty;
    }
}

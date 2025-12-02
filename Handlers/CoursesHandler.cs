#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TimeWarp.Nuru;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Cli.Handlers;

/// <summary>
/// Displays available courses using Nuru panel widget with paging support.
/// Courses are grouped by platform (Pluralsight, Dometrain, etc.).
/// </summary>
public static class CoursesHandler
{
    private const string CoursesJsonUrl = "https://ardalis.com/courses.json";
    private const string CoursesPageUrl = "https://ardalis.com/courses";

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    static CoursesHandler()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "ardalis-cli");
    }

    private static readonly Course[] FallbackCourses =
    [
        new Course
        {
            Name = "SOLID Principles for C# Developers",
            Link = "https://www.pluralsight.com/courses/csharp-solid-principles",
            Platform = "Pluralsight",
            Description = "Learn the SOLID principles of object-oriented design and how to apply them in C# to write maintainable, flexible code."
        },
        new Course
        {
            Name = "Getting Started: Modular Monoliths in .NET",
            Link = "https://dometrain.com/course/getting-started-modular-monoliths-in-dotnet/?ref=steve-ardalis-smith&coupon_code=ARDALIS",
            Platform = "Dometrain",
            Description = "A modular monolith breaks up the application into logical modules which are largely independent from one another. This provides many of the benefits of more distributed approaches like microservices without the overhead of deploying and managing a distributed application."
        }
    ];

    public static async Task ExecuteAsync(bool showAll, int pageSize)
    {
        ITerminal terminal = NuruTerminal.Default;

        terminal.WriteLine("Ardalis's Available Courses".Green().Bold());
        terminal.WriteLine();

        List<Course> courses;

        try
        {
            courses = await FetchCoursesFromUrlAsync();
        }
        catch
        {
            terminal.WriteLine("Using fallback course list...".Gray());
            terminal.WriteLine();
            courses = [.. FallbackCourses];
        }

        if (courses.Count == 0)
        {
            terminal.WriteLine("No courses available at the moment.".Yellow());
            return;
        }

        // Group courses by platform and flatten for display
        List<(string Platform, Course Course)> allCoursesToDisplay = courses
            .GroupBy(c => c.Platform ?? "Other")
            .OrderBy(g => g.Key)
            .SelectMany(g => g.Select(c => (g.Key, c)))
            .ToList();

        if (showAll)
        {
            // Display all courses without paging
            string? currentPlatform = null;
            foreach ((string platform, Course course) in allCoursesToDisplay)
            {
                DisplayPlatformHeaderIfChanged(terminal, platform, ref currentPlatform);
                DisplayCourse(terminal, course);
            }
        }
        else
        {
            // Display courses with paging
            int currentIndex = 0;
            string? currentPlatform = null;

            while (currentIndex < allCoursesToDisplay.Count)
            {
                int endIndex = Math.Min(currentIndex + pageSize, allCoursesToDisplay.Count);
                List<(string Platform, Course Course)> pageCourses = allCoursesToDisplay
                    .Skip(currentIndex)
                    .Take(endIndex - currentIndex)
                    .ToList();

                foreach ((string platform, Course course) in pageCourses)
                {
                    DisplayPlatformHeaderIfChanged(terminal, platform, ref currentPlatform);
                    DisplayCourse(terminal, course);
                }

                currentIndex = endIndex;

                if (currentIndex < allCoursesToDisplay.Count)
                {
                    terminal.Write("Press ".Gray());
                    terminal.Write("Space".Bold());
                    terminal.Write(" for more, or any other key to exit...".Gray());

                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    terminal.WriteLine();

                    if (key.Key != ConsoleKey.Spacebar)
                    {
                        terminal.WriteLine($"Showing {currentIndex} of {allCoursesToDisplay.Count} courses".Gray());
                        break;
                    }
                    terminal.WriteLine();
                }
            }
        }

        terminal.WriteLine();
        string coursesUrl = AddUtmSource(CoursesPageUrl);
        terminal.WriteLine("Learn more at: ".Gray() + CoursesPageUrl.Link(coursesUrl).Cyan());
    }

    private static void DisplayPlatformHeaderIfChanged(ITerminal terminal, string platform, ref string? currentPlatform)
    {
        if (currentPlatform != platform)
        {
            if (currentPlatform != null)
            {
                terminal.WriteLine();
            }
            terminal.WriteLine(platform.Cyan().Bold());
            terminal.WriteLine();
            currentPlatform = platform;
        }
    }

    private static void DisplayCourse(ITerminal terminal, Course course)
    {
        string urlWithTracking = AddUtmSource(course.Link);
        string displayUrl = StripQueryString(course.Link);

        string description = string.IsNullOrEmpty(course.Description)
            ? "No description available".Gray()
            : course.Description;

        string content =
            course.Name.Bold() + "\n\n" +
            description + "\n\n" +
            "Learn more: ".Gray() + displayUrl.Link(urlWithTracking).Cyan();

        terminal.WritePanel(panel => panel
            .Content(content)
            .Border(BorderStyle.Rounded)
            .BorderColor(AnsiColors.Green)
            .Padding(1, 0));

        terminal.WriteLine();
    }

    private static async Task<List<Course>> FetchCoursesFromUrlAsync()
    {
        string response = await HttpClient.GetStringAsync(CoursesJsonUrl);
        List<Course>? courses = JsonSerializer.Deserialize<List<Course>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return courses ?? [];
    }

    private sealed class Course
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}

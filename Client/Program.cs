using System.Diagnostics;
using System.Text;

namespace Client;

public class Program
{
    private const int MaxConcurrentRequests = 200;
    private static readonly TimeSpan DelayBetweenRequests = TimeSpan.FromMilliseconds(10);
    private static readonly string Payload = GeneratePayload();

    public static async Task Main(string[] args)
    {
        var url = args[0];

        using var timer = new PeriodicTimer(DelayBetweenRequests);
        using var semaphore = new SemaphoreSlim(MaxConcurrentRequests);

        while (await timer.WaitForNextTickAsync())
        {
            await semaphore.WaitAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await SendRequest(url);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }

    private static async Task SendRequest(string url)
    {
        using var client = new HttpClient();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var content = new StringContent(Payload);
            using var response = await client.PostAsync(url, content);

            Console.WriteLine($"{stopwatch.Elapsed} {response.StatusCode} {response.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{stopwatch.Elapsed} {ex.GetType().Name} {ex.Message}");
        }
    }

    private static string GeneratePayload()
    {
        var iterations = 100_000;
        var sb = new StringBuilder();

        for (int i = 0; i < iterations; ++i)
        {
            sb.Append("{\"A\":");
        }

        sb.Append("{}");

        for (int i = 0; i < iterations; ++i)
        {
            sb.Append('}');
        }

        return sb.ToString();
    }
}

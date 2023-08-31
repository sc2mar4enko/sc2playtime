using System.Reflection;
using s2protocol.NET;

namespace sc2playtime.scripts;

public static class ReplayAnalysis
{
    private static readonly string? AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static async ValueTask<(int, int)> GameLengthReturning(string replaysPath, string? assemblyPath = null)
    {
        assemblyPath ??= AssemblyPath;

        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Details = false,
            Metadata = true,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false
        };

        if (assemblyPath != null)
        {
            ReplayDecoder decoder = new(assemblyPath);
            CancellationTokenSource cts = new();
            var result = (0, 0);
            var replays = Directory.GetFiles(replaysPath, "*.SC2Replay");
            await foreach (var replay in decoder.DecodeParallel(replays, 2, options, cts.Token))
            {
                if (replay.Metadata != null)
                {
                    result.Item1++;
                    result.Item2 += replay.Metadata.Duration;
                }
            }

            cts.Cancel();
            cts.Dispose();
            decoder.Dispose();
            return result;
        }

        return (0,0);
    }
}
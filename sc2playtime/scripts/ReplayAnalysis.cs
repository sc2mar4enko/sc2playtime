using System.Reflection;
using IronPython;
using IronPython.Hosting;
using Microsoft.CodeAnalysis;
using s2protocol.NET;

namespace sc2playtime.scripts;

public class ReplayAnalysis
{
    public static readonly string? _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static async Task<int> GameLengthReturning(string filePath, string? assemblyPath = null)
    {
        if (assemblyPath == null)
        {
            assemblyPath = _assemblyPath;
        }

        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Details = true,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false
        };

        ReplayDecoder decoder = new(assemblyPath);

        Sc2Replay? replay = await decoder.DecodeAsync(filePath, options);
        if (replay != null && replay.Metadata != null) 
            return replay.Metadata.Duration;
        return 0;
    }

}
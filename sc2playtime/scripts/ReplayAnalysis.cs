using System.Reflection;
using IronPython;
using IronPython.Hosting;
using Microsoft.CodeAnalysis;
using s2protocol.NET;

namespace sc2playtime.scripts;

public class ReplayAnalysis
{
    public static async Task<int> GameLengthReturning(string filePath)
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var pythonLibPath = Path.Combine(assemblyPath, "Lib"); // Путь к библиотекам IronPython

        ReplayDecoder decoder = new(assemblyPath);
        //decoder.SearchPaths.Add(pythonLibPath); // Добавление пути к библиотекам
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Details = true,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false
        };
        Sc2Replay? replay = await decoder.DecodeAsync(filePath, options);
        return replay.Metadata.Duration;
    }
    
}
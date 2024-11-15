using System.Diagnostics;
using System.Text;

namespace DotNetPackager;

internal class Program
{
    private static string _dotnetPath = @"C:\Program Files\dotnet\dotnet.exe";
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
            return;
        _dotnetPath = Environment.OSVersion.Platform == PlatformID.Unix
            ? "/usr/local/share/dotnet/dotnet"
            : @"C:\Program Files\dotnet\dotnet.exe";
        var distinctArgs = args.Distinct().ToList();
        var results = distinctArgs.ToDictionary(arg => arg, _ => false);
        Console.WriteLine("Installing Packages...");
        foreach (var arg in distinctArgs)
            results[arg] = await AddPackageAsync(arg);
        Console.WriteLine("\n\n");
        PrintResults(results);
        Console.WriteLine("");
    }

    private static async Task<bool> AddPackageAsync(string arg)
    {
        try
        {
            Console.WriteLine(arg);
            var processArgs = $"add package {arg}";
            using var process = new Process();
            process.StartInfo = new(_dotnetPath, processArgs)
            {
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while adding the package.\n" +
            $"====================\n{ex.Message.Trim()}\n====================");
            return false;
        }
    }

    private static void PrintResults(Dictionary<string, bool> results)
    {
        var sb = new StringBuilder();
        foreach (var result in results)
            sb.AppendLine($"\t{result.Key}: {result.Value}");
        Console.WriteLine($"Here are the results:\n{sb}");
    }
}
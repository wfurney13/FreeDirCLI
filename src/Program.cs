using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace FreeDirCLI;

class Program
{
    public static bool prefersLightMode;
    public static bool diskSizesOnly;
    public static bool orderedOutput;
    public static bool allDisks;
    public static bool userContinued;

    static void Main(string[] args)
    {
        ArgumentParser.Run(args);
    }

    public static void DisplayResults(Dictionary<string, double> pairs)
    {
        double totalSize = 0;
        if (orderedOutput)
        {
            var orderedPairs = pairs
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var keyValuePair in orderedPairs)
            {
                Helper.Write(
                    $"{keyValuePair.Key, -45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );

                totalSize += keyValuePair.Value;
            }
        }
        else
        {
            foreach (var keyValuePair in pairs)
            {
                Helper.Write(
                    $"{keyValuePair.Key, -45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );

                totalSize += keyValuePair.Value;
            }
        }

        Helper.Write(
            $"\nUsed Space: {Math.Round(totalSize, 2)} GB\n\nCannot access {SizeGatherer.UnauthorizedAccessExceptionFileCount} files\n",
            ConsoleColor.Red,
            false
        );

        if (!allDisks)
        {
            if (!userContinued)
            {
                Helper.Write(
                    "Type New Path (:q to quit, {Up Arrow Key} to fill in previous path)",
                    ConsoleColor.Green,
                    prefersLightMode
                );
            }

            Helper.WriteInline("> ", ConsoleColor.Green, prefersLightMode);
            Helper.HandleReadLine(Console.ReadLine());
        }
    }
}

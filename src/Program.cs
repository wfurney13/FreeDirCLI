namespace FreeDirCLI;

class Program
{
    public static bool userContinued;
    public static DriveInfo[]? drives;

    static void Main(string[] args)
    {
        drives = DriveInfo.GetDrives();
        ArgumentParser.Run(args);
    }

    public static void DisplayResults(Dictionary<string, double> pairsTask)
    {
        double totalSize = 0;

        if (Config.orderedOutput)
        {
            var orderedPairs = pairsTask
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var keyValuePair in orderedPairs)
            {
                Writer.Write(
                    $"{keyValuePair.Key,-45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    Config.prefersLightMode
                );

                totalSize += keyValuePair.Value;
            }
        }
        else
        {
            foreach (var keyValuePair in pairsTask)
            {
                Writer.Write(
                    $"{keyValuePair.Key,-45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    Config.prefersLightMode
                );

                totalSize += keyValuePair.Value;
            }
        }

        if (totalSize > 1)
        {
            Writer.Write(
                $"\nUsed Space: {Math.Round(totalSize, 2)} GB\n\nCannot access {SizeGatherer.UnauthorizedAccessExceptionFileCount} files\n",
                ConsoleColor.Red,
                false
                );
        }
        else
        {
            Writer.Write(
                $"\nUsed Space: {Math.Round(totalSize, 4)} GB\n\nCannot access {SizeGatherer.UnauthorizedAccessExceptionFileCount} files\n",
                ConsoleColor.Red,
                false
                );
        }

        if (SizeGatherer.filePath != null)
        {
            Writer.Write(
                $"{SizeGatherer.filePath.ToUpper()}",
                ConsoleColor.Green,
                Config.prefersLightMode
            );
        }

        if (!userContinued)
        {
            Writer.Write(
                $"(:q to quit, :b to go back)",
                ConsoleColor.Green,
                Config.prefersLightMode
            );
        }

        if (!Config.allDisks)
        {
            Writer.WriteInline("> ", ConsoleColor.Green, Config.prefersLightMode);
            Writer.HandleReadLine(Console.ReadLine());
        }
    }
}

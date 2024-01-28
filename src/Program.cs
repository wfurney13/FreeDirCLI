using FreeDirCLI.Utility;

namespace FreeDirCLI;

class Program
{
    public static bool userContinued;
    public static DriveInfo[]? drives;
    public static List<string> DirectoryNames;

    static void Main(string[] args)
    {
        drives = DriveInfo.GetDrives();
        ArgumentParser.Run(args);
    }

    static void StoreResults(Dictionary<string, long> results)
    {
	DirectoryNames.Clear();
        var ResultsDict = results.Keys.ToList();

        foreach (var result in ResultsDict)
        {
            DirectoryNames.Add(result);
        }


        if (results.Count > 1000)
        {
            throw new NotImplementedException();
        }
    }

    static void WritePair(KeyValuePair<string, long> kvp)
    {
        string size = Converter.ConvertFromBytes(kvp.Value);
        string name = kvp.Key;
        if (name.Length > 40)
        {
            name = $"{name.Substring(0, 40)}...";
        }

        Writer.Write(
            $"{name,-45}\t{size}",
            ConsoleColor.Yellow,
            Config.prefersLightMode
        );
    }


    public static void DisplayResults(Dictionary<string, long> pairsTask)
    {
        StoreResults(pairsTask);
        double totalSize = 0;

        Dictionary<string, long> Pairs = new();

        if (Config.orderedOutput)
        {
            Pairs = pairsTask
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        if (!Config.orderedOutput)
        {
            Pairs = pairsTask
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        foreach (var keyValuePair in Pairs)
        {
            WritePair(keyValuePair);
            // convert byte to appropriate size
            totalSize += keyValuePair.Value / 1024d / 1024d / 1024d;
        }

        Writer.Write(
            $"\nUsed Space: {Math.Round(totalSize
                , 2)} GB\n\nCannot access {SizeGatherer.UnauthorizedAccessExceptionFileCount} files\n",
            ConsoleColor.Red,
            false
        );

        if (SizeGatherer.filePath != null)
        {
            Writer.Write(
                $"{SizeGatherer.filePath.ToUpper()}",
                ConsoleColor.Green,
                Config.prefersLightMode
            );
        }


        if (!Config.allDisks)
        {
            if (!userContinued)
            {
                Writer.Write(
                    $"(:q to quit, :b to go back)",
                    ConsoleColor.Green,
                    Config.prefersLightMode
                );
            }

            Writer.WriteInline("> ", ConsoleColor.Green, Config.prefersLightMode);
            Readline.ReadKey(Console.ReadKey(intercept: true));
        }
    }
}



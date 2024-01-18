﻿namespace FreeDirCLI;

class Program
{
    public static bool userContinued;
    public static DriveInfo[]? drives;

    static void Main(string[] args)
    {
        drives = DriveInfo.GetDrives();
        ArgumentParser.Run(args);
    }

    public static void DisplayResults(Dictionary<string, long> pairsTask)
    {
        double totalSize = 0;

        if (Config.orderedOutput)
        {
            var orderedPairs = pairsTask
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var keyValuePair in orderedPairs)
            {
                // convert byte to appropriate size
                string size = Converter.ConvertFromBytes(keyValuePair.Value);
                string name = keyValuePair.Key;
                if (name.Length > 40)
                {
                    name = $"{name.Substring(0, 40)}...";
                }
                Writer.Write(
                    $"{name,-45}\t{size}",
                    ConsoleColor.Yellow,
                    Config.prefersLightMode
                );

                totalSize += keyValuePair.Value /1024d /1024d/1024d;
            }
        }
        else
        {
            foreach (var keyValuePair in pairsTask)
            {
             // convert byte to appropriate size
                string size = Converter.ConvertFromBytes(keyValuePair.Value);
                string name = keyValuePair.Key;
                if (name.Length > 20)
                {
                    name = $"{name.Substring(0, 20)}...";
                }
                Writer.Write(
                    $"{name,-45}\t{size}",
                    ConsoleColor.Yellow,
                    Config.prefersLightMode
                );

                totalSize += keyValuePair.Value /1024d /1024d/ 1024d;
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

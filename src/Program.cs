using FreeDirCLI.Utility;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace FreeDirCLI;

class Program
{
    public static bool UserContinued;
    public static DriveInfo[]? Drives;
    public static List<string>? StoredResults;

    static void Main(string[] args)
    {
        Drives = DriveInfo.GetDrives();
        Config.GetSlashType();
        Config.CheckConfig(
              args); // Does the user have a config file already setup. If so use the information in the file. Unless they passed in '-config' again, then give them the option to recreate it

        if (args.Contains("-d"))
        {
            Config.DiskSizesOnly = true;
        }

        if (args.Contains("-h") || args.Contains("-help"))
        {
            Writer.DisplayHelpMessage();
        }
        else
        {

            ParseArgsAndSetFilePath(args);
            SizeGatherer.CheckForPathAndRun();
        }
    }

    static void ParseArgsAndSetFilePath(string[] args)
    {
        if (args.Length == 1)
        {
            if (!args[0].StartsWith(
                    "-")) // if there is only one argument and it is not -*. It should be a file path
            {
                SizeGatherer.FilePath = args[0];
            }
        }

        if (args.Length == 2)
        {
            if (!args[1].StartsWith("-")) // ensure args[1] is not a switch (and thus should be a file path)
            {
                SizeGatherer.FilePath = args[1];
            }
        }

    }

    static void StoreResults(Dictionary<string, long> results)
    {
        Debug.Assert(StoredResults is not null);

        if (StoredResults.Any())
        {
            StoredResults.Clear();
        }

        var resultsDict = results.Keys.ToList();

        foreach (var result in resultsDict)
        {
            StoredResults.Add(result);
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
            Config.PrefersLightMode
        );
    }


    public static void DisplayResults(Dictionary<string, long> pairs)
    {
        long totalSize = 0;
        if (pairs.Count > 0)
        {
        if (Config.OrderedOutput)
        {
            pairs = pairs
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        if (!Config.OrderedOutput)
        {
            pairs = pairs
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        StoreResults(pairs);

        if (!SizeGatherer.AllDrives)
        {
            for (int x = Console.CursorTop; x > 0; x--)
            {
                Console.SetCursorPosition(0, x);
                Console.Write(new string(' ', Console.BufferWidth));
            }
        }
        Writer.Write(
            $"{"Directory Name",-45}\tDirectory Size",
            ConsoleColor.Yellow,
            Config.PrefersLightMode
        );
        Writer.Write($"{"——————————————",-45}\t——————————————", ConsoleColor.Yellow, Config.PrefersLightMode);



        foreach (var keyValuePair in pairs)
        {
            WritePair(keyValuePair);
            // convert byte to appropriate size
            totalSize += keyValuePair.Value;
        }

        Writer.Write($"\nUsed Space: {Converter.ConvertFromBytes(totalSize)}", ConsoleColor.Red, false);

        if (SizeGatherer.UnauthorizedAccessExceptionFileCount > 0 && !SizeGatherer.AllDrives)
        {
            Writer.Write(
                $"\nCannot access {SizeGatherer.UnauthorizedAccessExceptionFileCount} files\nUse command :which to see list of files that cannot be accessed.",
                ConsoleColor.Red,
                false
            );
        }

        if (SizeGatherer.FilePath != null)
        {
            Writer.Write(
                $"\n{SizeGatherer.FilePath.ToUpper()}",
                ConsoleColor.Green,
                Config.PrefersLightMode
            );
        }


        if (!SizeGatherer.AllDrives)
        {
            if (!UserContinued)
            {
                Writer.Write(
                    $"(:q to quit, :b to go back)",
                    ConsoleColor.Green,
                    Config.PrefersLightMode
                );
            }

            UserContinuing();
        }
    }
        else 
        {
            Writer.Write("No files found for path. Enter valid file path.",ConsoleColor.Red, false);
            FilePathModifier.TrimFilePathBackOneLevel();
        }

        if (!SizeGatherer.AllDrives)
            UserContinuing();
    }

    public static void UserContinuing()
    {
        if (StoredResults == null || SizeGatherer.AllDrives)
        {
            StoredResults = new();
        }

        if (SizeGatherer.UnauthorizedFileList == null || SizeGatherer.AllDrives)
        {
            SizeGatherer.UnauthorizedFileList = new();
        }

        Writer.WriteInline("> ", ConsoleColor.Green, Config.PrefersLightMode);
        Readline.ReadKey(Console.ReadKey(intercept: true));
    }
}



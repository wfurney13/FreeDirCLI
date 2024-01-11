using System.Collections.Generic;

namespace FreeDirCLI;

class Program
{
    public static bool prefersLightMode;

    static void Main(string[] args)
    {
        SetPrefersLightMode(args);

        if (
            args.Length == 0
            || args[0] == "-l" && args.Length == 1
            || args[0] == "-o" && args.Length == 1
            || args.Contains("-l") && args.Contains("-o") && args.Length == 2
        )
        {
            GetSizeOfAllFolders(args);
        }

        if (args.Length == 1 && !prefersLightMode && args[0] != "-o")
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(args[0]);
            DisplayResults(args, nameAndSizePairs, true);
        }

        if (args.Length == 2 && prefersLightMode && args[1] != "-o" && args[1] != "-l")
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(args[1]);
            DisplayResults(args, nameAndSizePairs, true);
        }

        if (args.Length == 2 && !prefersLightMode)
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(args[1]);
            DisplayResults(args, nameAndSizePairs, true);
        }

        if (args.Length == 3 && prefersLightMode)
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(args[2]);
            DisplayResults(args, nameAndSizePairs, true);
        }
    }

    static void SetPrefersLightMode(string[] args)
    {
        if (args.Length > 0 && args.Contains("-l"))
        {
            prefersLightMode = true;
        }
    }

    static void Write(string message, ConsoleColor color, bool lightMode)
    {
        if (prefersLightMode == false)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        else //lightmode users do not get a fancy color
        {
            Console.WriteLine(message);
        }
    }

    static void GetSizeOfAllFolders(string[] args)
    {
        Write(
            "When no file path is provided as an argument, this tool will scan all of your drives, sum up the file sizes of each directory, and return the results. Would you like to use the tool on all drives?",
            ConsoleColor.Blue,
            prefersLightMode
        );
        string? n = Console.ReadLine();
        switch (n)
        {
            case "y":
            case "Y":
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    Write(
                        $"\nRetreiving info for {drive}...\n",
                        ConsoleColor.Blue,
                        prefersLightMode
                    );
                    Write(
                        $"Total Size: {Math.Round(drive.TotalSize / 1024d / 1024d / 1024d, 2)} GB\nFree Space: {Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d, 2)} GB\n",
                        ConsoleColor.Blue,
                        prefersLightMode
                    );
                    //fix this with ordering
                    var nameAndSizePairsForDrive = GetSizeOfEachFolderForPath(drive.ToString());
                    DisplayResults(args, nameAndSizePairsForDrive, true);
                }
                break;
            default:
                Write(
                    "Run this program with a file path as an argument.",
                    ConsoleColor.Blue,
                    prefersLightMode
                );
                break;
        }
    }

    static Dictionary<string, double> GetSizeOfEachFolderForPath(string filePath)
    {
        //create dictionary for holding directory name and sizes
        Dictionary<string, double> nameSizePairs = new();

        //create instance of DirectoryInfo for file path provided
        DirectoryInfo dirInfo = new(filePath);
        //create a directories variable that is the enumerable directories of that class
        var directories = TryEnumerateDirectories(dirInfo);
        Write(
            $"\n{"Directory Name", -45}\tDirectory Size (GB)\n",
            ConsoleColor.White,
            prefersLightMode
        );

        //loop through each directory at the path

        foreach (var dir in directories)
        {
            //try to get the dir size, if we dont have access to the dir just continue and let the user know
            try
            {
                //for each dir, we need to enumerate all the files within, and store the sum of those file sizes in the dirSize variable
                long dirSize = dir.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);

                //byte to GB coversion
                double GBDirSize = dirSize / 1024d / 1024d / 1024d;

                // //Write the name and file size sum of each directory to the console
                // Write(
                //     $"{dir.Name, -45}\t{Math.Round(GBDirSize, 4)}",
                //     ConsoleColor.Yellow,
                //     prefersLightMode
                // );

                nameSizePairs.Add(dir.Name, Math.Round(GBDirSize, 4));
            }
            catch (System.UnauthorizedAccessException) // no access to the dir
            {
                Write($"No access to dir: {dir.Name}", ConsoleColor.Red, prefersLightMode);
            }
        }
        return nameSizePairs;
    }

    static IEnumerable<DirectoryInfo> TryEnumerateDirectories(DirectoryInfo directoryInfo)
    {
        try
        {
            IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();
            return directories;
        }
        catch (DirectoryNotFoundException)
        {
            Write($"Provided file path is invalid\n", ConsoleColor.Red, false);
            throw;
        }
    }

    static void DisplayResults(string[] args, Dictionary<string, double> pairs, bool orderBySize)
    {
        if (args.Contains("-o"))
        {
            var orderedPairs = pairs
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var keyValuePair in orderedPairs)
            {
                Write(
                    $"{keyValuePair.Key, -45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );
            }
        }
        else
        {
            foreach (var keyValuePair in pairs)
            {
                Write(
                    $"{keyValuePair.Key, -45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );
            }
        }
    }
}

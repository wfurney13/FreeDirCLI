namespace FreeDirCLI;

class Program
{
    public static bool prefersLightMode;
    public static bool diskSizesOnly;
    public static bool orderedOutput;

    public static string? filePath;

    static void Main(string[] args)
    {
        if (args.Contains("-l"))
        {
            prefersLightMode = true;
        }

        if (args.Contains("-d"))
        {
            diskSizesOnly = true;
        }

        if (args.Contains("-o"))
        {
            orderedOutput = true;
        }

        if (args.Contains("-h") || args.Contains("-help"))
        {
            DisplayHelpMessage();
        }
        else
        {
            ParseArgsAndSetFilePath(args);
        }

        CheckForPathAndRun();
    }

    static void DisplayHelpMessage()
    {
        Write(
            $"See the readme.md file for more information about this program\n\nhttps://github.com/wfurney13/FreeDirCLI/blob/master/README.md",
            ConsoleColor.Red,
            false
        );
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

    static void ParseArgsAndSetFilePath(string[] args)
    {
        if (args.Length == 1)
        {
            if (!prefersLightMode && !diskSizesOnly && !orderedOutput) // if there is only one argument and it is not -h, -help, -d , -l or -o. It should be a file path
            {
                filePath = args[0];
            }
        }
        if (args.Length == 2)
        {
            if (args[1].Length > 2) // ensure args[1] is not a switch
            {
                filePath = args[1];
            }
        }
        if (args.Length == 3 && prefersLightMode) // the only situation where there should be three arguments is when prefersLightMode is true
        {
            filePath = args[2];
        }
    }

    static void CheckForPathAndRun()
    {
        if (filePath == null)
        {
            GetSizeOfAllFolders();
        }
        else
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(filePath);
            DisplayResults(nameAndSizePairs);
        }
    }

    static void GetSizeOfAllFolders()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            Write($"\nRetreiving info for {drive}...\n", ConsoleColor.Blue, prefersLightMode);
            Write(
                $"Total Size: {Math.Round(drive.TotalSize / 1024d / 1024d / 1024d, 2)} GB\nFree Space: {Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d, 2)} GB\n",
                ConsoleColor.Blue,
                prefersLightMode
            );
            if (!diskSizesOnly)
            {
                var nameAndSizePairsForDrive = GetSizeOfEachFolderForPath(drive.ToString());
                DisplayResults(nameAndSizePairsForDrive);
            }
        }
    }

    static Dictionary<string, double> GetSizeOfEachFolderForPath(string arg)
    {
        //create dictionary for holding directory name and sizes
        Dictionary<string, double> nameSizePairs = new();

        //create instance of DirectoryInfo for file path provided
        DirectoryInfo dirInfo = new(arg);
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

    static void DisplayResults(Dictionary<string, double> pairs)
    {
        if (orderedOutput)
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

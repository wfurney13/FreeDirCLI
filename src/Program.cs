namespace FreeDirCLI;

class Program
{
    public static bool prefersLightMode;
    public static bool diskSizesOnly;
    public static bool orderedOutput;

    public static string? filePath;

    static void Main(string[] args)
    {
        Config.CheckConfig(args); // if the user has a config file setup but passes in some of these arguments we want to choose what they passed in anyway instead of whats in the config file

        if (args.Contains("-l"))
        {
            prefersLightMode = !prefersLightMode; // prefersLightMode is initialized to false. If it is set to 'true' in the config, and -l is still passed in, we want to set it to false instead of true.
        }

        if (args.Contains("-d"))
        {
            diskSizesOnly = true;
        }

        if (args.Contains("-o"))
        {
            orderedOutput = !orderedOutput; // orderedOutput is initialized to false. If it is set to 'true' in the config, and -o is still passed in, we want to set it to false instead of true.
        }

        if (args.Contains("-h") || args.Contains("-help"))
        {
            Helper.DisplayHelpMessage();
        }
        else
        {
            ParseArgsAndSetFilePath(args);
            CheckForPathAndRun();
        }
    }

    static void ParseArgsAndSetFilePath(string[] args)
    {
        if (args.Length == 1)
        {
            if (!args[0].StartsWith("-")) // if there is only one argument and it is not -*. It should be a file path
            {
                filePath = args[0];
            }
        }
        if (args.Length == 2)
        {
            if (!args[1].StartsWith("-")) // ensure args[1] is not a supported switch (and should thus be a file path)
            {
                filePath = args[1];
            }
        }
        if (args.Length == 3) // if there are 3 arguments filePath should always be args[2]
        {
            filePath = args[2];
        }
    }

    static void CheckForPathAndRun()
    {
        if (filePath == null) // when there is no file path passed in we want to run for all folders
        {
            GetSizeOfAllFolders();
        }
        else // otherwise use the file path that was passed in
        {
            var nameAndSizePairs = GetSizeOfEachFolderForPath(filePath);
            DisplayResults(nameAndSizePairs);
        }
    }

    static void GetSizeOfAllFolders()
    {
        if (!diskSizesOnly)
        {
            Helper.Write("Run program for all drives?", ConsoleColor.Blue, prefersLightMode);

            string? getSizeOfAllFolderResponse = Console.ReadLine();

            if (getSizeOfAllFolderResponse == null)
            {
                return;
            }
            switch (getSizeOfAllFolderResponse.ToLower())
            {
                case "y":
                    break;
                case "n":
                    return;
                default:
                    return;
            }
        }

        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            Helper.Write(
                $"\nRetreiving info for {drive}...\n",
                ConsoleColor.Blue,
                prefersLightMode
            );
            Helper.Write(
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

    static Dictionary<string, double> GetSizeOfEachFolderForPath(string filePath)
    {
        //create dictionary for holding directory name and sizes
        Dictionary<string, double> nameSizePairs = new();

        //create instance of DirectoryInfo for file path provided
        DirectoryInfo dirInfo = new(filePath);

        //create a directories variable that is the enumerable directories of that class
        var directories = TryEnumerateDirectories(dirInfo);
        Helper.Write(
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
                Helper.Write($"No access to dir: {dir.Name}", ConsoleColor.Red, prefersLightMode);
            }
        }
        return nameSizePairs;
    }

    public static IEnumerable<DirectoryInfo> TryEnumerateDirectories(DirectoryInfo directoryInfo)
    {
        try
        {
            IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();
            return directories;
        }
        catch (DirectoryNotFoundException)
        {
            Helper.Write($"Provided file path is invalid\n", ConsoleColor.Red, false);
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
                Helper.Write(
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
                Helper.Write(
                    $"{keyValuePair.Key, -45}\t{keyValuePair.Value}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );
            }
        }
    }
}

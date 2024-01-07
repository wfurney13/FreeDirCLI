using System.ComponentModel;

namespace FreeDirCLI;

class Program
{
    public static bool prefersLightMode;

    static void Main(string[] args)
    {
        SetPrefersLightMode(args);

        if (args.Length == 0 || args[0] == "-l" && args.Length == 1)
        {
            GetSizeOfAllFolders();
        }

        if (args.Length == 1 && !prefersLightMode)
        {
            GetSizeOfEachFolderForPath(args[0]);
        }

        if (args.Length == 2 && prefersLightMode)
        {
            GetSizeOfEachFolderForPath(args[1]);
        }
    }

    static void SetPrefersLightMode(string[] args)
    {
        if (args.Length > 0 && args[0] == "-l")
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

    static void GetSizeOfAllFolders()
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
                    GetSizeOfEachFolderForPath(drive.ToString());
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

    static void GetSizeOfEachFolderForPath(string filePath)
    {
        //create instance of DirectoryInfo for file path provided
        DirectoryInfo dirInfo = new(filePath);
        //create a directories variable that is the enumerable directories of that class
        IEnumerable<DirectoryInfo> directories = dirInfo.EnumerateDirectories();

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

                //Write the name and file size sum of each directory to the console
                Write(
                    $"{dir.Name, -45}\t{Math.Round(GBDirSize, 4)}",
                    ConsoleColor.Yellow,
                    prefersLightMode
                );
            }
            catch (System.UnauthorizedAccessException) // no access to the dir
            {
                Write($"No access to dir: {dir.Name}", ConsoleColor.Red, prefersLightMode);
            }
        }
    }
}

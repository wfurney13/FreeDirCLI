namespace FreeDirCLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 1) // could be > 0 later if more args are added
        {
            GetSizeOfEachFolderAsync(args);
        }
        else
        {
            Write("Run the program with a filepath string as an argument.", ConsoleColor.Red);
        }
    }

    static void Write(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void GetSizeOfEachFolderAsync(string[] args)
    {
        //create instance of DirectoryInfo for file path provided
        DirectoryInfo dirInfo = new DirectoryInfo(args[0]);
        //create a directories variable that is the enumerable directories of that class
        IEnumerable<DirectoryInfo> directories = dirInfo.EnumerateDirectories();

        Write($"{"Directory Name", -45}\tDirectory Size (GB)", ConsoleColor.White);

        //loop through each directory at the path
        foreach (var x in directories)
        {
            //try to get the dir size, if we dont have access to the dir just continue and let the user know
            try
            {
                //for each dir, we need to enumerate all the files within, and store the sum of those file sizes in the dirSize variable
                long dirSize = x.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);

                //byte to GB coversion
                double GBDirSize = dirSize / 1024d / 1024d / 1024d;
                //Write the name and file size sum of each directory to the console
                Write($"{x.Name, -45}\t{Math.Round(GBDirSize, 4)}", ConsoleColor.Yellow);
            }
            catch (System.UnauthorizedAccessException) // no access to the dir
            {
                Write($"No access to dir: {x.Name}", ConsoleColor.Red);
            }
        }
    }
}

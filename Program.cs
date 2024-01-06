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
        // use the file system to recurse through each directory item and measure the length of each child item in each of those directories, sum it up, and attach it to the directory item that is returned
        DirectoryInfo dirInfo = new DirectoryInfo(args[0]);
        var directories = dirInfo.EnumerateDirectories();

        Write($"{"Directory Name".PadRight(45)}\tDirectory Size (GB)", ConsoleColor.White);

        foreach (var x in directories)
        {
            //get the dir size, if we dont have access to the dir just continue and let the user know
            try
            {
                long dirSize = x.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);
                double GBDirSize = dirSize / 1024d / 1024d / 1024d;
                Write($"{x.Name, -45}\t{Math.Round(GBDirSize, 4)}", ConsoleColor.Yellow);
            }
            catch (System.UnauthorizedAccessException)
            {
                Write($"No access to dir: {x.Name}", ConsoleColor.Red);
            }
        }
    }
}

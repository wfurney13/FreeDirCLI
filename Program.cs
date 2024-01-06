using System.Dynamic;
using Microsoft.VisualBasic.FileIO;

namespace FreeDirCLI;

class Program
{
    static async Task Main(string[] args)
    {
        if (args == null)
        {
            throw new NotImplementedException();
        }
        //need to implement no args

        // args[0] will be the file path string

        if (args[0] == "-s") { }

        if (args[0] != "-s")
        {
            await GetSizeOfEachFolderAsync(args[0]);
        }
    }

    static void Write(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void Write(double message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static async Task GetSizeOfEachFolderAsync(string filePath)
    {
        // use the file system to recurse through each directory item and measure the length of each child item in each of those directories, sum it up, and attach it to the directory item that is returned
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        var directories = dirInfo.EnumerateDirectories();

        Write($"{"Directory Name".PadRight(45)}\tDirectory Size (GB)", ConsoleColor.White);

        foreach (var x in directories)
        {
            //get the dir size, if we dont have access to the dir just continue and let the user know
            try
            {
                long dirSize = await Task.Run(
                    () =>
                        x.EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                            .Sum(file => file.Length)
                );
                double GBDirSize = dirSize / 1024d / 1024d / 1024d;
                Write($"{x.Name.PadRight(45)}\t{Math.Round(GBDirSize, 4)}", ConsoleColor.Yellow);
            }
            catch (System.UnauthorizedAccessException)
            {
                Write($"No access to dir: {x.Name}", ConsoleColor.Red);
            }
        }
    }
}

using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks.Dataflow;

namespace FreeDirCLI
{
    public class SizeGatherer
    {
        public static int UnauthorizedAccessExceptionFileCount;
        public static string? filePath;
        public static List<Dictionary<string, double>>? previousResults;

        public static void CheckForPathAndRun()
        {
            if (filePath == null) // when there is no file path passed in we want to run for all folders
            {
                GetSizeOfAllFolders();
            }
            else // otherwise use the file path that was passed in
            {
                var nameAndSizePairs = GetSizeOfEachFolderForPath(filePath);
                Program.DisplayResults(nameAndSizePairs);
            }
        }

        public static void GetSizeOfAllFolders()
        {
            if (!Config.diskSizesOnly)
            {
                Writer.Write(
                    "Run program for all drives? y/n",
                    ConsoleColor.Blue,
                    Config.prefersLightMode
                );

                string? getSizeOfAllFolderResponse = Console.ReadLine();

                if (getSizeOfAllFolderResponse == null)
                {
                    Writer.DisplayHelpMessage();
                    return;
                }
                switch (getSizeOfAllFolderResponse.ToLower())
                {
                    case "y":
                        Config.allDisks = true;
                        break;
                    default:
                        Writer.DisplayHelpMessage();
                        return;
                }
            }

            if (Program.drives != null)
            {
                foreach (var drive in Program.drives)
                {
                    Writer.Write(
                        $"\nRetreiving info for {drive}...\n",
                        ConsoleColor.Blue,
                        Config.prefersLightMode
                        );
                    Writer.Write(
                        $"Total Size: {Math.Round(drive.TotalSize / 1024d / 1024d / 1024d,2)} GB\nFree Space: {Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d),2} GB\n",
                        ConsoleColor.Blue,
                        Config.prefersLightMode
                        );
                    if (!Config.diskSizesOnly)
                    {
                        var nameAndSizePairsForDrive = GetSizeOfEachFolderForPath(drive.ToString());
                        Program.DisplayResults(nameAndSizePairsForDrive);
                    }
                }
            }
        }

        public static Dictionary<string, double> GetSizeOfEachFolderForPath(string filePath)
        {
            //create dictionary for holding directory name and sizes
            Dictionary<string, double> nameSizePairs = new();

            //create instance of DirectoryInfo for file path provided
            DirectoryInfo dirInfo = new(filePath);

            var directories = TryEnumerateDirectories(dirInfo);

            foreach (var dir in directories)
            {
                //try to get the dir size, if we dont have access to the dir just continue and let the user know
                try
                {
                    long dirSize = new FileInfoEnumerable(
                        dir
                    ).Sum(fileLength => fileLength);

                    //byte to GB coversion
                    double GBDirSize = dirSize / 1024d / 1024d / 1024d;

                    if (GBDirSize < 1)
                    {
                        nameSizePairs.Add(dir.Name, Math.Round(GBDirSize,4));
                    }
                    else
                    {
                        nameSizePairs.Add(dir.Name, Math.Round(GBDirSize,2));
                    }


                }
                catch (System.UnauthorizedAccessException) // no access to the dir
                {
                    Writer.Write(
                        $"No access to dir: {dir.Name}",
                        ConsoleColor.Red,
                        Config.prefersLightMode
                    );
                }
            }

            Writer.ClearLastLine();

            Writer.Write(
                $"\n\n{"Directory Name", -45}\tDirectory Size (GB)\n",
                ConsoleColor.White,
                Config.prefersLightMode
            );

            return nameSizePairs;
        }

        public static IEnumerable<DirectoryInfo> TryEnumerateDirectories(
            DirectoryInfo directoryInfo
        )
        {
            try
            {
                IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();
                return directories;
            }
            catch (DirectoryNotFoundException)
            {
                Writer.Write(
                    $"Provided file path ({directoryInfo.FullName}) is invalid\n",
                    ConsoleColor.Red,
                    false
                );
                throw;
            }
        }
    }
}

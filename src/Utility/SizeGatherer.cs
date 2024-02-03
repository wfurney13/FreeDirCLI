using FreeDirCLI.Utility;
using System.Diagnostics;

namespace FreeDirCLI
{
    public class SizeGatherer
    {
        public static int UnauthorizedAccessExceptionFileCount;
        public static List<string>? UnauthorizedFileList;
        public static string? FilePath;
        public static bool AllDrives;

        public static void CheckForPathAndRun()
        {     
            if (FilePath == null) // when there is no file path passed in we want to run for all folders
            {
                GetSizeOfAllFolders();
            }
            else // otherwise use the file path that was passed in
            {
                FilePathModifier.ValidatePath();

                var nameAndSizePairs = GetSizeOfEachFolderForPath(FilePath);
                Program.DisplayResults(nameAndSizePairs);
            }
        }

        public static void GetSizeOfAllFolders()
        {
            if (!Config.DiskSizesOnly)
            {
                Writer.Write(
                    "Run program for all drives? y/n",
                    ConsoleColor.Blue,
                    Config.PrefersLightMode
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
                            AllDrives = true;
                            Writer.Write(
                            "The application will now loop through your drives and determine the size of each parent directory. Press CTRL+C to exit the application at any time.",
                            ConsoleColor.Blue,
                            Config.PrefersLightMode
                        );                     
                        break;
                    default:
                        Console.Clear();
                        //Writer.DisplayHelpMessage();
                        Writer.Write("\nEnter File Path:");
                        Program.UserContinuing();
                        return;
                }
            }

            if (Program.Drives != null)
            {
                foreach (var drive in Program.Drives)
                {
                    Writer.Write(
                        $"\nRetreiving info for {drive}...\n",
                        ConsoleColor.Blue,
                        Config.PrefersLightMode
                    );
                    Writer.Write(
                        $"Total Size: {Math.Round(drive.TotalSize / 1024d / 1024d / 1024d, 2)} GB\nFree Space: {Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d),2} GB\n",
                        ConsoleColor.Blue,
                        Config.PrefersLightMode
                    );
                    if (!Config.DiskSizesOnly)
                    {
                        var nameAndSizePairsForDrive = GetSizeOfEachFolderForPath(drive.ToString());
                        Program.DisplayResults(nameAndSizePairsForDrive);
                    }
                }

                Writer.Write(
                    $"(:q to quit, :b to go back)",
                    ConsoleColor.Green,
                    Config.PrefersLightMode
                );
                Program.UserContinuing();
            }
            
        }

        public static Dictionary<string, long> GetSizeOfEachFolderForPath(string filePath)
        {

            if (Program.UserContinued == false)
            {
                Program.StoredResults = new();
                UnauthorizedFileList = new();
            }

            Debug.Assert(UnauthorizedFileList != null);
            UnauthorizedFileList.Clear();
            UnauthorizedAccessExceptionFileCount = 0;

            //create dictionary for holding directory name and sizes
            Dictionary<string, long> nameSizePairs = new();

            //create instance of DirectoryInfo for file path provided
            DirectoryInfo dirInfo = new(filePath);

            var directories = TryEnumerateDirectories(dirInfo);

            try
            {
                foreach (var dir in directories)
                {
                    //try to get the dir size, if we dont have access to the dir just continue and let the user know
                    try
                    {
                        long dirSize = new FileInfoEnumerable(
                            dir
                        ).Sum(file => file.Length);

                        //byte to GB coversion
                        nameSizePairs.Add(dir.Name, dirSize);
                    }
                    catch (System.UnauthorizedAccessException) // no access to the dir
                    {
                        Writer.Write(
                            $"No access to dir: {dir.Name}",
                            ConsoleColor.Red,
                            Config.PrefersLightMode
                        );
                    }
                }
            }
            catch (Exception)
            {
                Writer.Write($"Error enumerating directories at path {filePath}. Enter a new path:", ConsoleColor.Red, false);
                FilePathModifier.TrimFilePathBackOneLevel();
                Program.UserContinuing();
            }

            // try to enumerate files at the parent as well

            try
            {
                var files = dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    long fileSize = file.Length;
                    nameSizePairs.Add(file.Name, fileSize);
                }
            }
            catch (UnauthorizedAccessException)
            {
                SizeGatherer.UnauthorizedAccessExceptionFileCount++;
            }

            return nameSizePairs;
        }

        public static IEnumerable<DirectoryInfo> TryEnumerateDirectories(
            DirectoryInfo directoryInfo
        )
        {
            try
            {
                IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();
                List<string> dnames = new();


                return directories;
            }
            catch (Exception)
            {
                FilePathModifier.InvalidPathResponse();
                Environment.Exit(0);
                throw;
            }
        }
    }
}


namespace FreeDirCLI
{
    public class SizeGatherer
    {
        public static int UnauthorizedAccessExceptionFileCount;
        public static string? filePath;

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
            if (!Program.diskSizesOnly)
            {
                Helper.Write(
                    "Run program for all drives? y/n",
                    ConsoleColor.Blue,
                    Program.prefersLightMode
                );

                string? getSizeOfAllFolderResponse = Console.ReadLine();

                if (getSizeOfAllFolderResponse == null)
                {
                    Helper.DisplayHelpMessage();
                    return;
                }
                switch (getSizeOfAllFolderResponse.ToLower())
                {
                    case "y":
                        Program.allDisks = true;
                        break;
                    default:
                        Helper.DisplayHelpMessage();
                        return;
                }
            }

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                Helper.Write(
                    $"\nRetreiving info for {drive}...\n",
                    ConsoleColor.Blue,
                    Program.prefersLightMode
                );
                Helper.Write(
                    $"Total Size: {Math.Round(drive.TotalSize / 1024d / 1024d / 1024d, 2)} GB\nFree Space: {Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d, 2)} GB\n",
                    ConsoleColor.Blue,
                    Program.prefersLightMode
                );
                if (!Program.diskSizesOnly)
                {
                    var nameAndSizePairsForDrive = GetSizeOfEachFolderForPath(drive.ToString());
                    Program.DisplayResults(nameAndSizePairsForDrive);
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

            //loop through each directory at the path
            Helper.WriteInline("Loading...");

            foreach (var dir in directories)
            {
                //try to get the dir size, if we dont have access to the dir just continue and let the user know
                try
                {
                    long dirSize = new FileInfoEnumerable(
                        dir,
                        "*",
                        SearchOption.AllDirectories
                    ).Sum(file => file.Length);

                    //byte to GB coversion
                    double GBDirSize = dirSize / 1024d / 1024d / 1024d;

                    nameSizePairs.Add(dir.Name, Math.Round(GBDirSize, 4));
                }
                catch (System.UnauthorizedAccessException) // no access to the dir
                {
                    Helper.Write(
                        $"No access to dir: {dir.Name}",
                        ConsoleColor.Red,
                        Program.prefersLightMode
                    );
                }
            }

            Helper.Write(
                $"\n\n{"Directory Name", -45}\tDirectory Size (GB)\n",
                ConsoleColor.White,
                Program.prefersLightMode
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
                Helper.Write($"Provided file path is invalid\n", ConsoleColor.Red, false);
                throw;
            }
        }
    }
}

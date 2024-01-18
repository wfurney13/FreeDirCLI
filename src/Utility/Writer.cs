namespace FreeDirCLI;

public class Writer
{
    public static void Write(string message, ConsoleColor color, bool lightMode)
    {
        if (Config.prefersLightMode == false)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        else
        {
            Write(message);
        }
    }

    public static void Write(string message)
    {
        Console.WriteLine(message);
    }

    public static void HandleReadLine(string? consoleResponse)
    {
        bool newPath = false;

        while (consoleResponse == "")
        {
            WriteInline("> ", ConsoleColor.Green, Config.prefersLightMode);
            consoleResponse = Console.ReadLine();
        }

        if (consoleResponse == ":q")
        {
            return;
        }

        if (consoleResponse != null)
        {
            Program.userContinued = true;

            try
            {
                if (Program.drives != null)
                {
                    //if the user passes in a new file path starting with one of their drives we know its a whole new path
                    foreach (var drive in Program.drives)
                    {
                        if (consoleResponse.ToUpper().StartsWith(drive.ToString().ToUpper()))
                        {
                            SizeGatherer.filePath = consoleResponse;
                            newPath = true;
                        }
                    }
                }

                if (consoleResponse.ToLower() == ":b" || consoleResponse.ToLower() == "back")
                {
                    TrimFilePathBackOneLevel();
                }
                //else assume what is passed to the consoleResponse will be a partial file path that is one of the options displayed in the results try it as a full file path as well if it errors
                else if (
                    SizeGatherer.filePath != null
                    && SizeGatherer.filePath.EndsWith($"{Config.slashType}")
                    && newPath == false
                )
                {
                    SizeGatherer.filePath += $"{consoleResponse}";
                    AddSlashToFilePath();
                }
                else if (newPath == false)
                {
                    AddSlashToFilePath();
                }

                if (SizeGatherer.filePath != null)
                {
                    var nameAndSizePairs = SizeGatherer.GetSizeOfEachFolderForPath(
                        SizeGatherer.filePath
                    );
                    Program.DisplayResults(nameAndSizePairs);
                }
            }
            catch (Exception)
            {
                WriteInline("> ", ConsoleColor.Green, Config.prefersLightMode);
                TrimFilePathBackOneLevel();
                Write($"{SizeGatherer.filePath}");
                HandleReadLine(Console.ReadLine());
            }
        }
    }

    public static void TrimFilePathBackOneLevel()
    {
        if (SizeGatherer.filePath != null && !SizeGatherer.filePath.EndsWith($"{Config.slashType}"))
        {
            SizeGatherer.filePath += $"{Config.slashType}";
        }

        // this is absolutely terrible and could be done with regex but is a hack "for now"...
        if (SizeGatherer.filePath != null && SizeGatherer.filePath.EndsWith($"{Config.slashType}"))
        {
            SizeGatherer.filePath = SizeGatherer.filePath.Remove(SizeGatherer.filePath.Length - 1);
            while (!SizeGatherer.filePath.EndsWith($"{Config.slashType}")) //lmao wtf
            {
                SizeGatherer.filePath = SizeGatherer.filePath.Remove(
                    SizeGatherer.filePath.Length - 1 //ridiculous but works
                );
            }
        }
    }

    public static void AddSlashToFilePath()
    {
        if (
            SizeGatherer.filePath != null
            && Config.slashType != null
            && !SizeGatherer.filePath.EndsWith(Config.slashType)
        )
        {
            SizeGatherer.filePath += Config.slashType;
        }
    }

    public static void WriteInline(string message, ConsoleColor color, bool lightMode)
    {
        if (Config.prefersLightMode == false)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        else
        {
            WriteInline(message);
        }
    }

    public static void WriteInline(string message)
    {
        Console.Write(message);
    }

    public static void DisplayHelpMessage()
    {
        Write(
            $"See the readme.md file for more information about this program\n\nhttps://github.com/wfurney13/FreeDirCLI/blob/master/README.md",
            ConsoleColor.Red,
            false
        );
    }
    
}

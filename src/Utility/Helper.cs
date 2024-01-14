namespace FreeDirCLI;

public class Helper
{
    public static void Write(string message, ConsoleColor color, bool lightMode)
    {
        if (Program.prefersLightMode == false)
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
        while (consoleResponse == "")
        {
            Helper.WriteInline("> ", ConsoleColor.Green, Program.prefersLightMode);
            consoleResponse = Console.ReadLine();
        }

        if (consoleResponse == ":q")
        {
            return;
        }

        if (consoleResponse != null)
        {
            Program.userContinued = true;

            string slashType;

            if (Config.isWindows)
            {
                slashType = "\\";
            }
            else
            {
                slashType = "/";
            }
            //assume what is passed to the consoleResponse will be a partial file path that is one of the options displayed in the results try it as a full file path as well if it errors

            try
            {
                if (consoleResponse.ToLower() == "back")
                {
                    TrimFilePathBackOneLevel(slashType);
                }
                else if (
                    SizeGatherer.filePath != null && SizeGatherer.filePath.EndsWith($"{slashType}")
                )
                {
                    SizeGatherer.filePath += $"{consoleResponse}";
                }
                else if (
                    SizeGatherer.filePath != null && !SizeGatherer.filePath.EndsWith($"{slashType}")
                )
                {
                    SizeGatherer.filePath += $"{slashType}{consoleResponse}{slashType}";
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
                Helper.WriteInline("> ", ConsoleColor.Green, Program.prefersLightMode);
                TrimFilePathBackOneLevel(slashType);
                Write($"{SizeGatherer.filePath}");
                Helper.HandleReadLine(Console.ReadLine());
            }
        }
    }

    public static void TrimFilePathBackOneLevel(string slashType)
    {
        if (SizeGatherer.filePath != null && !SizeGatherer.filePath.EndsWith($"{slashType}"))
        {
            SizeGatherer.filePath += $"{slashType}";
        }
        // this is absolutely terrible and could be done with regex but is a hack "for now"...
        if (SizeGatherer.filePath != null && SizeGatherer.filePath.EndsWith($"{slashType}"))
        {
            SizeGatherer.filePath = SizeGatherer.filePath.Remove(SizeGatherer.filePath.Length - 1);
            while (!SizeGatherer.filePath.EndsWith($"{slashType}")) //lmao wtf
            {
                SizeGatherer.filePath = SizeGatherer.filePath.Remove(
                    SizeGatherer.filePath.Length - 1 //ridiculous but works
                );
            }
        }
    }

    public static void WriteInline(string message, ConsoleColor color, bool lightMode)
    {
        if (Program.prefersLightMode == false)
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

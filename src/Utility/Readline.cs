using System.Text;

namespace FreeDirCLI.Utility;

public class Readline
{
    static StringBuilder sb = new();

    public static void Read(string? consoleResponse)
    {
        bool newPath = false;

        if (consoleResponse == "")
        {
            while (consoleResponse == "")
            {
                Writer.WriteInline("> ", ConsoleColor.Green, Config.prefersLightMode);
                consoleResponse = Console.ReadLine();
            }
        }


        if (consoleResponse == ":q")
        {
            Environment.Exit(0);
        }

        if (consoleResponse.ToLower() == ":b" || consoleResponse.ToLower() == "back")
        {
            FilePathWorker.TrimFilePathBackOneLevel();
        }

        else if (consoleResponse != null)
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

            //else assume what is passed to the consoleResponse will be a partial file path that is one of the options displayed in the results try it as a full file path as well if it errors
            if (
                SizeGatherer.filePath != null
                && SizeGatherer.filePath.EndsWith($"{Config.slashType}")
                && newPath == false
            )
            {
                SizeGatherer.filePath += $"{consoleResponse}";
                FilePathWorker.AddSlashToFilePath();
            }

            if (newPath == false)
            {
                FilePathWorker.AddSlashToFilePath();
            }
        }

        Program.userContinued = true;

        if (SizeGatherer.filePath != null)
        {
            var nameAndSizePairs = SizeGatherer.GetSizeOfEachFolderForPath(
                SizeGatherer.filePath
            );
            Program.DisplayResults(nameAndSizePairs);
        }
    }

    public static void ReadKey(ConsoleKeyInfo keyPressed)
    {
        while (keyPressed.Key != ConsoleKey.Enter)
        {
            if (keyPressed.Key == ConsoleKey.DownArrow)
            {
                Console.WriteLine("Down Arrow Pressed");
            }

            else if (keyPressed.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Writer.WriteInline("\b \b");
                }
            }
            //tab completion (first match)
            else if (keyPressed.Key == ConsoleKey.Tab)
            {
                string builtString = sb.ToString();
                foreach (string name in Program.DirectoryNames)
                {
                    if (name.ToUpper().StartsWith(builtString.ToUpper()))
                    {
                        for (int x = 0; x < builtString.Length; x++)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Remove(sb.Length - 1, 1);
                                Writer.WriteInline("\b \b");
                            }
                        }

                        sb.Append(name);
                        Writer.WriteInline(name);
                        break;
                    }
                }
            }
            else
            {
                sb.Append(keyPressed.KeyChar.ToString());
                Writer.WriteInline(sb[sb.Length - 1].ToString());
            }

            ReadKey(Console.ReadKey(intercept: true));
        }

        if (keyPressed.Key == ConsoleKey.Enter)
        {
            var builtString = sb.ToString();
            sb.Remove(0, sb.Length);
            Read(builtString);
        }
    }
}

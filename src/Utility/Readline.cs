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
                Readline.ReadKey(Console.ReadKey(intercept: true));
            }
        }


        if (consoleResponse == ":q")
        {
            Environment.Exit(0);
        }

        if (consoleResponse != null && (consoleResponse.ToLower() == ":b" || consoleResponse.ToLower() == "back"))
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
            if (keyPressed.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Writer.WriteInline("\b \b");
                }
            }
            //arrow keys
            else if (keyPressed.Key == ConsoleKey.LeftArrow)
            {
                sb.Remove(0, sb.Length);
                Read(":b");
            }
            else if (keyPressed.Key == ConsoleKey.RightArrow)
            {
                ConsoleKeyInfo enterKey = new('\u0000', ConsoleKey.Enter, false, false, false);
                ReadKey(enterKey);
            }

            else if (keyPressed.Key == ConsoleKey.UpArrow)
            {
                string builtString = sb.ToString();
                if (builtString.Length > 0)
                {
                    //if there is no direct match, find the closet match for the string (press tab)
                    foreach (var name in Program.DirectoryNames)
                    {
                        if (name.ToUpper() != builtString.ToUpper() && name.ToUpper().StartsWith(builtString.ToUpper()))
                        {
                            ConsoleKeyInfo tabKey = new('\u0000', ConsoleKey.Tab, false, false, false);
                            ReadKey(tabKey);
                        }
                        //if there is a direct match, go "up" to the previous result. If there is a direct match and no previous result, go "up" to the last result
                        else if (name.ToUpper() == builtString.ToUpper())
                        {
                            //remove the current input
                            for (int x = 0; x < builtString.Length; x++)
                            {
                                if (sb.Length > 0)
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                    Writer.WriteInline("\b \b");
                                }
                            }

                            builtString = name;
                        }
                    }

                    if (Program.DirectoryNames.Contains(builtString))
                    {
                        int index = Program.DirectoryNames.FindIndex(x => x.StartsWith(builtString));
                        //top of the list, go to the bottom
                        if (index != 0)
                        {
                            index = index - 1;
                        }

                        Writer.WriteInline(Program.DirectoryNames[index]);
                        sb.Append(Program.DirectoryNames[index]);
                    }
                }
                else // there is no string and they hit up arrow, start at bottom of the list
                {
                    Writer.WriteInline(Program.DirectoryNames[^1]);
                    sb.Append(Program.DirectoryNames[^1]);
                }
            }

            else if (keyPressed.Key == ConsoleKey.DownArrow)
            {
                string builtString = sb.ToString();
                if (builtString.Length > 0)
                {
                    //if there is no direct match, find the closet match for the string (press tab)
                    foreach (var name in Program.DirectoryNames)
                    {
                        if (name.ToUpper() != builtString.ToUpper() && name.ToUpper().StartsWith(builtString.ToUpper()))
                        {
                            ConsoleKeyInfo tabKey = new('\u0000', ConsoleKey.Tab, false, false, false);
                            ReadKey(tabKey);
                        }
                        //if there is a direct match, go "up" to the previous result. If there is a direct match and no previous result, go "up" to the last result
                        else if (name.ToUpper() == builtString.ToUpper())
                        {
                            //remove the current input
                            for (int x = 0; x < builtString.Length; x++)
                            {
                                if (sb.Length > 0)
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                    Writer.WriteInline("\b \b");
                                }
                            }

                            builtString = name;
                        }
                    }

                    if (Program.DirectoryNames.Contains(builtString))
                    {
                        int index = Program.DirectoryNames.FindIndex(x => x.StartsWith(builtString));
                        //top of the list, go to the bottom
                        if (index != Program.DirectoryNames.Count - 1)
                        {
                            index = index + 1;
                        }

                        Writer.WriteInline(Program.DirectoryNames[index]);
                        sb.Append(Program.DirectoryNames[index]);
                    }
                }
                else // there is no string and they hit up arrow, start at bottom of the list
                {
                    Writer.WriteInline(Program.DirectoryNames[0]);
                    sb.Append(Program.DirectoryNames[0]);
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
            else if (keyPressed.Key == ConsoleKey.Escape)
            {
                string builtString = sb.ToString();
                //remove the current input
                for (int x = 0; x < builtString.Length; x++)
                {
                    if (builtString.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Writer.WriteInline("\b \b");
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

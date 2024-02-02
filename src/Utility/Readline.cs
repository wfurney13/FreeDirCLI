using System.Text;
using System.Diagnostics;
using System.IO;

namespace FreeDirCLI.Utility;

public class Readline
{
    static StringBuilder sb = new();

    public static void Read(string? consoleResponse)
    {
        Writer.Write("\n");
        bool newPath = false;

        if (consoleResponse == "")
        {
            while (consoleResponse == "")
            {
                ReadKey(Console.ReadKey(intercept: true));
            }
        }
        Debug.Assert(consoleResponse != null);
        if (consoleResponse.StartsWith(":"))
        {
            if (consoleResponse == ":q")
            {
                Environment.Exit(0);
            }
            if (consoleResponse == ":which")
            {
                Debug.Assert(SizeGatherer.UnauthorizedFileList != null);
            foreach (var file in SizeGatherer.UnauthorizedFileList)
            {
                Writer.Write($"\n{file}");
            }
            Writer.WriteInline("> ", ConsoleColor.Green, Config.PrefersLightMode);
            ReadKey(Console.ReadKey(intercept: true));
            }

            if (consoleResponse == ":b")
            {

                FilePathModifier.TrimFilePathBackOneLevel();

                Program.UserContinued = true;

                Debug.Assert(SizeGatherer.FilePath != null);

                var nameAndSizePairs = SizeGatherer.GetSizeOfEachFolderForPath(
                        SizeGatherer.FilePath
                        );
                Program.DisplayResults(nameAndSizePairs);
            }
        }


        else
        {
          
            Debug.Assert(Program.StoredResults != null);

            if (Program.Drives != null)
            {
                //if the user passes in a new file path starting with one of their drives we know its a whole new path
                foreach (var drive in Program.Drives)
                {
                    if (!Program.StoredResults.Contains(consoleResponse)
                        && consoleResponse.ToUpper()[0] == drive.ToString().ToUpper()[0]) // This would catch things like C: and C as drives and eventually ValidateFilePath will convert it to C:\
                    {
                        SizeGatherer.FilePath = consoleResponse;
                        newPath = true;
                    }

                }
            }

            if (SizeGatherer.FilePath == null) 
            {
                Writer.Write(
                    "Provided file path is invalid. Enter a valid file path. For example, C:\\\n",
                    ConsoleColor.Red,
                    false
                );
                Writer.WriteInline("> ", ConsoleColor.Green, Config.PrefersLightMode);
                Readline.ReadKey(Console.ReadKey(intercept: true));
            }

            Debug.Assert(SizeGatherer.FilePath != null);
                //else assume what is passed to the consoleResponse will be a partial file path that is one of the options displayed in the results try it as a full file path as well if it errors
            if (
                SizeGatherer.FilePath.EndsWith($"{Config.SlashType}")
                && newPath == false
                )
            {
                SizeGatherer.FilePath += $"{consoleResponse}{Config.SlashType}";            }

             if (!SizeGatherer.FilePath.EndsWith($"{Config.SlashType}")
                && newPath == false
                )
            {
                SizeGatherer.FilePath += $"{Config.SlashType}{consoleResponse}{Config.SlashType}";
            }

            Program.UserContinued = true;

            FilePathModifier.ValidatePath();

            var nameAndSizePairs = SizeGatherer.GetSizeOfEachFolderForPath(
                    SizeGatherer.FilePath
                    );
                Program.DisplayResults(nameAndSizePairs);
        }
    }

    public static void ReadKey(ConsoleKeyInfo keyPressed)
    {
        while (keyPressed.Key != ConsoleKey.Enter)
        {
            Debug.Assert(Program.StoredResults != null);
            
           
            if (keyPressed.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Writer.WriteInline("\b \b");
                }
            }
            //arrow keys
            else if (keyPressed.Key == ConsoleKey.LeftArrow && Program.StoredResults.Count > 0)
            {
                sb.Remove(0, sb.Length);
                Read(":b");
            }
            else if (keyPressed.Key == ConsoleKey.RightArrow && Program.StoredResults.Count > 0)
            {
                ConsoleKeyInfo enterKey = new('\u0000', ConsoleKey.Enter, false, false, false);
                ReadKey(enterKey);
            }

            else if (keyPressed.Key == ConsoleKey.UpArrow && Program.StoredResults.Count > 0)
            {
                string builtString = sb.ToString();
                if (builtString.Length > 0)
                {
                    //if there is no direct match, find the closet match for the string (press tab)
                    
                    foreach (var name in Program.StoredResults)
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

                    if (Program.StoredResults.Contains(builtString))
                    {
                        int index = Program.StoredResults.FindIndex(x => x.StartsWith(builtString));
                        //top of the list, go to the bottom
                        if (index != 0)
                        {
                            index = index - 1;
                        }

                        Writer.WriteInline(Program.StoredResults[index]);
                        sb.Append(Program.StoredResults[index]);
                    }
                }
                else // there is no string and they hit up arrow, start at bottom of the list
                {
                    Debug.Assert(Program.StoredResults != null);
                    Writer.WriteInline(Program.StoredResults[^1]);
                    sb.Append(Program.StoredResults[^1]);
                }
            }

            else if (keyPressed.Key == ConsoleKey.DownArrow && Program.StoredResults.Count > 0)
            {
                string builtString = sb.ToString();
                if (builtString.Length > 0)
                {
                    //if there is no direct match, find the closet match for the string (press tab)
                    Debug.Assert(Program.StoredResults != null);
                    foreach (var name in Program.StoredResults)
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

                    if (Program.StoredResults.Contains(builtString))
                    {
                        int index = Program.StoredResults.FindIndex(x => x.StartsWith(builtString));
                        //top of the list, go to the bottom
                        if (index != Program.StoredResults.Count - 1)
                        {
                            index = index + 1;
                        }

                        Writer.WriteInline(Program.StoredResults[index]);
                        sb.Append(Program.StoredResults[index]);
                    }
                }
                else // there is no string and they hit up arrow, start at bottom of the list
                {
                    Debug.Assert(Program.StoredResults != null);
                    Writer.WriteInline(Program.StoredResults[0]);
                    sb.Append(Program.StoredResults[0]);
                }
            }
            //tab completion (first match)
            else if (keyPressed.Key == ConsoleKey.Tab && Program.StoredResults.Count > 0)
            {
                string builtString = sb.ToString();

                Debug.Assert(Program.StoredResults != null);

                foreach (string name in Program.StoredResults)
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
            if (sb.Length > 0)
            {
                var builtString = sb.ToString();
                sb.Clear();
                Read(builtString);
            }
            else
            {
                ReadKey(Console.ReadKey(intercept: true)); 
            }
        }
    }


}

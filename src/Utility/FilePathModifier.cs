using System.Diagnostics;
using System.IO;

namespace FreeDirCLI.Utility;

public class FilePathModifier
{
    public static void TrimFilePathBackOneLevel()
    {
        Debug.Assert(SizeGatherer.FilePath != null);

            if (!SizeGatherer.FilePath.EndsWith($"{Config.SlashType}"))
            {
                SizeGatherer.FilePath += $"{Config.SlashType}";
            }

            // this is absolutely terrible and could be done with regex but is a hack "for now"...
            if (SizeGatherer.FilePath.EndsWith($"{Config.SlashType}"))
            {
                SizeGatherer.FilePath = SizeGatherer.FilePath.Remove(SizeGatherer.FilePath.Length - 1);
                while (!SizeGatherer.FilePath.EndsWith($"{Config.SlashType}")) //lmao wtf
                {
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Remove(
                        SizeGatherer.FilePath.Length - 1 //ridiculous but works
                    );
                }
            }

            ValidatePath();     
        
    }

    public static void ValidatePath()
    {
        Debug.Assert(SizeGatherer.FilePath != null);
        Debug.Assert(Config.SlashType != null);

        if (SizeGatherer.FilePath.Length <= 3
            && !SizeGatherer.FilePath.EndsWith(Config.SlashType))
        {
            switch (SizeGatherer.FilePath[^1])
            {
                case '\\':
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Replace("\\", Config.SlashType);
                    break;
                case '/':
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Replace("/",Config.SlashType);
                    break;
                case ':':
                    SizeGatherer.FilePath += Config.SlashType;
                    break;
                case ';':
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Replace(";",$":{Config.SlashType}");
                    break;
                default:
                    SizeGatherer.FilePath += $":{Config.SlashType}";
                    break;
            }
        }

        if (
            SizeGatherer.FilePath.Length > 3
            && (OperatingSystem.IsWindows() && SizeGatherer.FilePath.Contains('/')
            || !OperatingSystem.IsWindows() && SizeGatherer.FilePath.Contains('\\'))
            )
        {
            switch (Config.SlashType)
            {
                case "\\":
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Replace("/", Config.SlashType);
                    break;
                case "/":
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Replace("\\", Config.SlashType);
                    break;
                default:
                    break;
            }
        }

        if (!SizeGatherer.FilePath.EndsWith(Config.SlashType))
        {
            SizeGatherer.FilePath += Config.SlashType;
        }

    }

    public static void InvalidPathResponse()
    {
        Writer.Write(
                    $"Provided file path ({SizeGatherer.FilePath}) is invalid\n",
                    ConsoleColor.Red,
                    false
                );
        if (SizeGatherer.FilePath != null && SizeGatherer.FilePath.Length > 5 && SizeGatherer.FilePath.Contains($":{Config.SlashType}"))
        {
            TrimFilePathBackOneLevel();
        }
        else
        {
            SizeGatherer.FilePath = "";
        }
        Writer.WriteInline("> ", ConsoleColor.Green, Config.PrefersLightMode);
        Readline.ReadKey(Console.ReadKey(intercept: true));
    }

    

}
using System.Diagnostics;
using System.IO;

namespace FreeDirCLI.Utility;

public class FilePathModifier
{
    public static void TrimFilePathBackOneLevel()
    {
        ValidatePath();

        Debug.Assert(SizeGatherer.FilePath != null);
        if (SizeGatherer.FilePath.Length == 3 &&
            SizeGatherer.FilePath.EndsWith($"{Config.SlashType}"))
        {
            // reset the errored files and do nothing
            SizeGatherer.UnauthorizedAccessExceptionFileCount = 0;
        }
        else
        {
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
        }

        
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


}
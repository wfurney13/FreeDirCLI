using System.Diagnostics;

namespace FreeDirCLI.Utility;

public class FilePathModifier
{
    public static void TrimFilePathBackOneLevel()
    {
        Debug.Assert(SizeGatherer.FilePath != null);
        if (SizeGatherer.FilePath.Length == 3 &&
            SizeGatherer.FilePath.EndsWith($"{Config.slashType}"))
        {
            // reset the errored files and do nothing
            SizeGatherer.UnauthorizedAccessExceptionFileCount = 0;
        }
        else
        {
            if (!SizeGatherer.FilePath.EndsWith($"{Config.slashType}"))
            {
                SizeGatherer.FilePath += $"{Config.slashType}";
            }

            // this is absolutely terrible and could be done with regex but is a hack "for now"...
            if (SizeGatherer.FilePath.EndsWith($"{Config.slashType}"))
            {
                SizeGatherer.FilePath = SizeGatherer.FilePath.Remove(SizeGatherer.FilePath.Length - 1);
                while (!SizeGatherer.FilePath.EndsWith($"{Config.slashType}")) //lmao wtf
                {
                    SizeGatherer.FilePath = SizeGatherer.FilePath.Remove(
                        SizeGatherer.FilePath.Length - 1 //ridiculous but works
                    );
                }
            }
        }
    }


}
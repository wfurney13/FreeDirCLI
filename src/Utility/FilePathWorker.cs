namespace FreeDirCLI.Utility;

public class FilePathWorker
{
    public static void TrimFilePathBackOneLevel()
    {
        if (SizeGatherer.filePath.Length == 3 && SizeGatherer.filePath.EndsWith($"{Config.slashType}"))
        {
            // reset the errored files and do nothing
            SizeGatherer.UnauthorizedAccessExceptionFileCount = 0;
        }
        else
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
}
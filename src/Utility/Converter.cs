namespace FreeDirCLI;

public class Converter {
    public static string ConvertFromBytes(long size)
    {
        switch (size)
        {
            //gb
            case >= 1000000000:
                return $"{Math.Round(size /1024d /1024d /1024d,2).ToString()} GB";
            //mb
            case >= 1000000:
                return $"{Math.Round(size /1024d /1024d,2).ToString()} MB";
            default:
                return  $"{Math.Round(size /1024d,2).ToString()} KB";
                    
        }
    }
}
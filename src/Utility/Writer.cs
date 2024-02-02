namespace FreeDirCLI;

public class Writer
{
    public static void Write(string message, ConsoleColor color, bool lightMode)
    {
        if (Config.PrefersLightMode == false)
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


    public static void WriteInline(string message, ConsoleColor color, bool lightMode)
    {
        if (Config.PrefersLightMode == false)
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

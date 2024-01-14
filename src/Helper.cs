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
        else //lightmode users do not get a fancy color
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
        if (Program.prefersLightMode == false)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        else //lightmode users do not get a fancy color
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

namespace FreeDirCLI;

public class Config
{
    public static string configPath = Environment.ExpandEnvironmentVariables(
        @"%userprofile%\Documents\FreeDirCLI\fd.config"
    );

    public static void CheckConfig(string[] args)
    {
        if (args.Contains("-config"))
        {
            if (File.Exists(configPath))
            {
                Helper.Write(
                    $"Configuration file already exists. Would you like to delete and recreate the config file? y/n",
                    ConsoleColor.White,
                    true
                );
                string? recreateConfigFileResponse = Console.ReadLine();
                if (recreateConfigFileResponse == null)
                {
                    return;
                }
                switch (recreateConfigFileResponse.ToString().ToLower())
                {
                    case "y":
                        CreateConfigFile();
                        break;
                    case "n":
                        break;
                    default:
                        break;
                }
            }

            if (!File.Exists(configPath))
            {
                CreateConfigFile();
            }
        }

        if (File.Exists(configPath))
        {
            SetConfigChoices();
        }
    }

    static void CreateConfigFile()
    {
        string directoryPath = Environment.ExpandEnvironmentVariables(
            @"%userprofile%\Documents\FreeDirCLI\"
        ); // I dont think this will work for linux?
        if (File.Exists(configPath))
        {
            File.Delete(configPath);
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        Helper.Write("Would you like to default to light mode? y/n");
        string? lightmodeConfigResponse = Console.ReadLine();
        if (lightmodeConfigResponse == null)
        {
            return;
        }
        switch (lightmodeConfigResponse.ToString().ToLower())
        {
            case "y":
                File.AppendAllLines(configPath, new[] { $"prefersLightMode = 1" });
                break;
            case "n":
                File.AppendAllLines(configPath, new[] { $"prefersLightMode = 0" });
                break;
            default:
                break;
        }
        Helper.Write("Would you like to default output to ordered by size descending? y/n");
        string? orderedOutputConfigResponse = Console.ReadLine();
        if (orderedOutputConfigResponse == null)
        {
            return;
        }
        switch (orderedOutputConfigResponse.ToString().ToLower())
        {
            case "y":
                File.AppendAllLines(configPath, new[] { $"orderedOutput = 1" });
                break;
            case "n":
                File.AppendAllLines(configPath, new[] { $"orderedOutput = 0" });
                break;
            default:
                break;
        }

        Helper.Write("Config file created.", ConsoleColor.Yellow, true);
    }

    static void SetConfigChoices()
    {
        IEnumerable<string> lines = File.ReadLines(configPath);

        foreach (string line in lines)
        {
            switch (line)
            {
                case "prefersLightMode = 1":
                    Program.prefersLightMode = true;
                    break;
                case "prefersLightMode = 0":
                    Program.prefersLightMode = false;
                    break;
                case "orderedOutput = 1":
                    Program.orderedOutput = true;
                    break;
                case "orderedOutput = 0":
                    Program.orderedOutput = false;
                    break;
            }
        }
    }
}

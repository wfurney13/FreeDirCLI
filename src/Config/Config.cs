namespace FreeDirCLI;

class Config
{
    public static string? configPath;
    public static string? configDirectory;
    public static bool prefersLightMode;
    public static bool diskSizesOnly;
    public static bool orderedOutput;
    public static bool allDisks;
    public static string? slashType;

    static void CreateConfigFile()
    {
        if (configPath == null || configDirectory == null)
        {
            throw new DirectoryNotFoundException(
                "Could not set configPath or configDirectory for environment"
            );
        }

        if (File.Exists(configPath))
        {
            File.Delete(configPath);
        }

        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        Writer.Write("Would you like to default to light mode? y/n");
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

        Writer.Write("Would you like to default output to ordered by size descending? y/n");
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

        Writer.Write($"Config file created at {configPath}.", ConsoleColor.Yellow, true);
    }

    static void ConfigRecreate()
    {
        Writer.Write(
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
            default:
                break;
        }
    }

    static void SetConfigPath()
    {
        if (OperatingSystem.IsWindows())
        {
            configPath = Environment.ExpandEnvironmentVariables(
                @"%userprofile%\Documents\FreeDirCLI\config.txt"
            );
            configDirectory = Environment.ExpandEnvironmentVariables(
                @"%userprofile%\Documents\FreeDirCLI\"
            );
        }

        if (!OperatingSystem.IsWindows())
        {
            configPath = Environment.ExpandEnvironmentVariables(@"%HOME%/.fd_config");
            configDirectory = Environment.ExpandEnvironmentVariables(@"%HOME%/");
        }

        if (configPath == null || configDirectory == null)
        {
            throw new DirectoryNotFoundException(
                "Could not set configPath or configDirectory for environment"
            );
        }
    }

    static void SetConfigChoices()
    {
        if (configPath == null || configDirectory == null)
        {
            throw new DirectoryNotFoundException(
                "Could not set configPath or configDirectory for environment"
            );
        }

        IEnumerable<string> lines = File.ReadLines(configPath);

        foreach (string line in lines)
        {
            switch (line)
            {
                case "prefersLightMode = 1":
                    prefersLightMode = true;
                    break;
                case "prefersLightMode = 0":
                    prefersLightMode = false;
                    break;
                case "orderedOutput = 1":
                    orderedOutput = true;
                    break;
                case "orderedOutput = 0":
                    orderedOutput = false;
                    break;
            }
        }
    }

    public static void CheckConfig(string[] args)
    {
        SetConfigPath();

        if (args.Contains("-config"))
        {
            if (File.Exists(configPath))
            {
                ConfigRecreate();
            }

            if (!File.Exists(configPath))
            {
                CreateConfigFile();
            }

            if (configPath != null)
            {
                Writer.Write(configPath);
            }
        }

        if (File.Exists(configPath))
        {
            SetConfigChoices();
        }
    }

    public static void GetSlashType()
    {
        if (OperatingSystem.IsWindows())
        {
            slashType = "\\";
        }
        else
        {
            slashType = "/";
        }
    }
}

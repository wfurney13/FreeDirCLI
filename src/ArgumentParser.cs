using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeDirCLI
{
    public class ArgumentParser
    {
        public static void Run(string[] args)
        {
            Config.CheckConfig(args); // Does the user have a config file already setup. If so use the information in the file. Unless they passed in '-config' again, then give them the option to recreate it

            if (args.Contains("-l"))
            {
                Program.prefersLightMode = !Program.prefersLightMode; // if prefersLightMode = true in the config file, when -l is provided, we want prefersLightMode = false. Otherwise (or if there is no config file) prefersLightMode = false, and when -l is provided, it will flip it to true
            }

            if (args.Contains("-d"))
            {
                Program.diskSizesOnly = true;
            }

            if (args.Contains("-o"))
            {
                Program.orderedOutput = !Program.orderedOutput; // orderedOutput is initialized to false. If it is set to 'true' in the config, and -o is still passed in, we want to set it to false instead of true.
            }

            if (args.Contains("-h") || args.Contains("-help"))
            {
                Helper.DisplayHelpMessage();
            }
            else
            {
                ParseArgsAndSetFilePath(args);
                SizeGatherer.CheckForPathAndRun();
            }
        }

        static void ParseArgsAndSetFilePath(string[] args)
        {
            if (args.Length == 1)
            {
                if (!args[0].StartsWith("-")) // if there is only one argument and it is not -*. It should be a file path
                {
                    SizeGatherer.filePath = args[0];
                }
            }
            if (args.Length == 2)
            {
                if (!args[1].StartsWith("-")) // ensure args[1] is not a switch (and thus should be a file path)
                {
                    SizeGatherer.filePath = args[1];
                }
            }
            if (args.Length == 3) // if there are 3 arguments program.filePath should always be args[2]
            {
                SizeGatherer.filePath = args[2];
            }
        }
    }
}

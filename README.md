<h1>FreeDirCLI</h1>

![image](https://i.ibb.co/k0RM9f3/Screenshot-2024-01-07-152010.png)

<h2>Usage</h2>

I use this tool to determine which directories have files that use the most space since this info is not provided natively in the windows file explorer.

There are two main ways to use this tool. First, you can run it with a 'file path' argument, such as `C:\` or `C:\Users`. Alternatively you can run the tool with no arguments.

As this tool is a CLI, it is designed to run from the command line. All of the below examples are using powershell.

<h2>Examples</h2>

-- clone the repo

`git clone https://github.com/wfurney13/FreeDirCLI/`

-- change directory to the 'bin' folder

`cd .\FreeDirCLI\bin\`

-- Run the program with a file path argument

`.\fd.exe C:\`

-- Add quotes if your file path contains a space

`.\fd.exe 'C:\Program Files (x86)\'`

This will get the names of the subdirectories at that path, add up their file sizes, and return the name and sum. When no file path is provided as an argument, this tool will scan all of your drives, sum up the file sizes of each directory, and return the results.

-- Call the executable without a file path argument

`.\fd.exe`

There is also a lightmode switch ('-l'). For lightmode users or those who do not want colored text in the output, -l should be passed as the first argument.

-- Call the executable without a file path argument and with the light mode switch

`.\fd.exe -l`

-- Call the executable with a file path argument and the light mode switch

`.\fd.exe -l C:\`

<h2>Optional Build and PATH instructions</h2>

<h3>Build from source (.NET 7.0 required)</h3>

`git clone https://github.com/wfurney13/FreeDirCLI/`

`cd .\FreeDirCLI\src\`

`dotnet build`

-- FreeDirCLI.exe will be created at 

`~\FreeDirCLI\src\bin\Debug\net7.0\FreeDirCLI.exe`

I rename the exe to 'fd.exe', move it and `FreeDirCLI.dll` to the `\FreeDirCLI\bin\` folder.

<h3>PATH</h3>

You can also add the filepath of the executable to your PATH environment variable.

-- Add the filepath to powershell profile's PATH environment variable

`notepad $profile`

-- Add this line and save

`$env:PATH += "C:\PATH_TO_CLONED_REPO\FreeDirCLI\bin\"`

Then the program can be called with a simple

`fd`
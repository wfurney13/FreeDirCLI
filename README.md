<h1>FreeDirCLI</h1>

![image](/screenshots/regular.png)

<h2>Usage</h2>

I use this tool to determine which directories have files that use the most space since this info is not provided natively in the windows file explorer.

![image](https://i.ibb.co/T4jZDQv/Screenshot-2024-01-11-174203.png)

There are two main ways to use this tool. First, you can run it with a 'file path' argument, such as `C:\` or `C:\Users`. Alternatively you can run the tool with no arguments, or with one of the available switches (see below).

As this tool is a CLI, it is designed to run from the command line. All of the below examples are using powershell.

<h2>Examples</h2>

-- Download the binary for your OS (Windows or Linux)

-- Open your shell, change directory to the path that you have saved the executable to (alternatively see instructions below for adding the executable to the PATH)

`cd C:\bin\`

-- Run without a file path argument

`.\fd.exe`

-- Run the program with a file path argument

`.\fd.exe C:\`

-- Add quotes if your file path contains a space

`.\fd.exe 'C:\Program Files (x86)\'`

This will get the names of the subdirectories at that path, add up their file sizes, and return the name and sum. When no file path is provided as an argument, this tool will scan all of your drives, sum up the file sizes of each directory, and return the results. Then you can continue by typing the name of the next folder, file path, or :b to go to the parent folder.

![image](/screenshots/nextfolder.png)

<h2>Available Switches</h2>

* Lightmode (-l)

`.\fd.exe -l`

`.\fd.exe -l C:\`

* Order by size descending (-o)

`.\fd.exe -o`

`.\fd.exe -o C:\`

`.\fd.exe -l -o C:\`

![image](/screenshots/orderedlightmode.png)

* Show Drive info only (-d)

`.\fd.exe -d`

`.\fd.exe -l -d`

![image](/screenshots/drives.png)

* Configuration (-config)

Allows you to save choices for '-l' or '-o'

`.\fd.exe -config`

For Windows, the config file will be saved at:

`%userprofile%\Documents\FreeDirCLI\config.txt`

For Linux, the config file will be saved at:

`%HOME%/.fd_config`

<h2>Optional Build and PATH instructions</h2>

<h3>Build from source (.NET 7.0 required)</h3>

`git clone https://github.com/wfurney13/FreeDirCLI/`

`cd .\FreeDirCLI\src\`

<h3>Windows</h3>

`dotnet build -o ..\bin\Windows\`

-- FreeDirCLI.exe file will be created at 

`~\FreeDirCLI\bin\Windows\FreeDirCLI.exe`

-- To publish only the self-contained executable at C:\bin\

`dotnet publish -o C:\bin\ -r win-x64`

<h3>Linux</h3>

`dotnet build -o ../bin/Linux/`

-- FreeDirCLI file will be created at 

`~/FreeDirCLI/bin/Linux/FreeDirCLI`

-- To publish only the self-contained executable at ~/bin/

`dotnet publish -o ~/bin/ -r linux-x64`


<h3>Add to PATH Instructions</h3>

<h3>Windows</h3>

-- Add the filepath to powershell profile's PATH environment variable

`notepad $profile`

-- Add this line and save (replace PATH_TO_REPO_BIN with the path to the bin\Windows\ folder that the executable is in)

`$env:PATH += "C:\PATH_TO_REPO_BIN\"`

Restart powershell, then the program can be called with a simple

`fd`

<h3>Linux</h3>

-- Example for bash

`vi ~/.bashrc`

`export PATH=$PATH:/PATH_TO_REPO_BIN/Linux`

Then source your ~/.bashrc

`source ~/.bashrc`

Then the program can be called with a simple

`fd`
Call the executable with a file path argument

Example:
.\FreeDirCLI.exe C:\

Or

.\FreeDirCLI.exe 'C:\Program Files (x86)\'

This will get the names of the subdirectories at that path (only one level deep currently), add up their file sizes, and return the name and sum 

Build instructions (.net 7.0 required):

`git clone https://github.com/wfurney13/FreeDirCLI/`

`cd .\FreeDirCLI\`

`dotnet build -c Release`

FreeDirCLI.exe will be created at 

`~\FreeDirCLI\bin\Release\net7.0\FreeDirCLI.exe`

I use this tool for finding out which directories are using the most space since this info is not provided natively in the windows file explorer.

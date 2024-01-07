FreeDirCLI

![image](https://i.ibb.co/k0RM9f3/Screenshot-2024-01-07-152010.png)

I use this tool for finding out which directories are using the most space since this info is not provided natively in the windows file explorer. This allows me to quickly determine which directories I may need to target for freeing up space. 

There are two main ways to use this tool. First, you can run it with a 'file path' argument, such as C:\ or C:\Users. Alternatively you can run the tool with no arguments.

As this tool is a CLI, it is designed to run from the command line. All of the below examples are using powershell.

Usage:
-- clone the repo
`git clone https://github.com/wfurney13/FreeDirCLI/`

-- change directory to the 'bin' folder
`cd .\FreeDirCLI\bin\`

-- Run the program with a file path argument
`.\fd.exe C:\`

-- Add quotes if your file path contains a space
`.\fd.exe 'C:\Program Files (x86)\'`

This will get the names of the subdirectories at that path, add up their file sizes, and return the name and sum 

When no file path is provided as an argument, this tool will scan all of your drives, sum up the file sizes of each directory, and return the results.


-- Call the executable without a file path argument
`.\fd.exe`


There is also a lightmode switch ('-l'). For lightmode users or those who do not want colored text in the output, -l should be passed as the first argument.

-- Call the executable without a file path argument and with the light mode switch
`.\fd.exe -l`

-- Call the executable with a file path argument and the light mode switch
`.\fd.exe -l C:\`


Optional Build instructions (.net 7.0 required):

`git clone https://github.com/wfurney13/FreeDirCLI/`

`cd .\FreeDirCLI\src\`

`dotnet build -c Release`

FreeDirCLI.exe will be created at 
`~\FreeDirCLI\src\bin\Release\net7.0\FreeDirCLI.exe`

I rename the exe to 'fd.exe' and then add the file path to my PATH environment variable

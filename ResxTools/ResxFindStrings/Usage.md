/? - This help

/allfiles:patern - File pattern  for finding all Resx files (*.resx)

/trans:patern - File pattern for finding all translated Resx files (*.??*.resx)

/lang:patern - A file pattern for language-specific codes for finding target files

/src:folder - Root folder where all Resx files reside

/out:pathname - Output path for the To-Be-Translated XML report file

ID - One or more unique Resx string IDs (name)

Argslist.txt - A program arguments file that lists one or more command line arguments.
 - The file name can be any name as long as it does not have a Resx file extension. 
 - Each line is treated as a command line arg.
 - Comment (#) and blank lines are ignored.
 - End-of-Line comments are removed.
 - All lines are trimmed to remove leading and trailing spaces.

Windows Wildcard File Search
 - Asterisk (*): Matches any number of characters, including zero.
 - Question mark (?): Matches a single character.
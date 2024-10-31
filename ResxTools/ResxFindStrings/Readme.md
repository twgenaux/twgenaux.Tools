

# Resx Find Strings To-Be-Translated

The primary purpose of this program is to export Resx strings that have been identified for translation. The Resx string are written out to an XML file along with their source references. 

```
Finds Resx strings for translation
Usage:
  ResxFindStrings.exe /src:<root-folder> /allfiles:<patern>... /trans:<patern>... /lang:<patern>... /id:<name>... <argslist.txt>
  Program args:
  
/? - This help

/allfiles:<patern> - One or more file wildcard pattern for finding all Resx files (*.resx)

/trans:<patern> - One or more file wildcard pattern for finding all translated Resx files (*.??*.resx)

/lang:<patern> - One or more file wildcard pattern for finding language-specific codes for finding target files (*.en.resx)

/src:<root-folder> - Where all Resx files reside

/out:pathname - Output path for the To-Be-Translated XML report file

/id:<name> - One or more unique Resx string IDs (name)

argslist.txt - One or more program arguments file that contains one or more command line arguments.
 - The file name can be any name.
 - Each line is treated as a command line arg (see above).
 - Comment (#) and blank lines are ignored.
 - End-of-Line comments are removed.
 - All lines are trimmed to remove leading and trailing spaces.

Windows Wildcard File Search
 - Asterisk (*): Matches any number of characters, including zero.
 - Question mark (?): Matches a single character.

```

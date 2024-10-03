This is an example Program Args file.

- Each line is treated as a command line arg.
- Comment (#) and blank lines are ignored.
- End-of-Line comments are removed.
- All lines are trimmed to remove leading and trailing spaces.

Windows Wildcard File Search

- Asterisk (`*`): Matches any number of characters, including zero.
- Question mark (`?`): Matches a single character.

Examples:

`*.resx` : Matches all files with a `.resx` extension.

`*.??*.resx` : Matches all files with language codes of two characters or more.

`*.fr.resx` :  Matches files with the French language code.

File Patterns
1. To find strings in source (untranslated) Resx files:
    - Include `/lang:*.en.resx` with the file pattern with a source language code.
    If no language code is included in the source Resx filename, then:
    - Include `/allfiles:*.resx` with the file pattern of all Resx files.
    - Include `/trans:*.??*.resx` with the file pattern of all translated Resx files.
    The tool will create a list of All-Files and Translated-Files. It will then
    remove the translated files from the list of All-Files, leaving only the source files.
2. To search individual language Resx files. Include a file pattern for each language:
   `/lang:*.fr.resx`
   `/lang:*.ja.resx`
   /`lang:*.zh-CN.resx`
3. To find strings in only one source Resx file:
   `/lang:C:\ResxFiles\Customer.fr.resx`

   Resx string IDs (name), provide a list of the IDs:
   `/lang:*.fr.resx`
   `/lang:*.ja.resx`
   `/lang:*.zh-CN.resx`

### Example - no language code is included in source filenames
`/allfiles:*.resx`
`/trans:*.??*.resx`
`/src:E:\Translations.ResX` # Source folder

`/out:Example_Resx_Strings_To-Be-Translated.xml` # XML To-Be-Translated list

List of the Resx identifiers to be translated
`CassetteIndexIDColon`
`StopProcessing`
`SolError.Apsw19.Short`
`SolError.Apsw19.Long`

## xmldoc2md

Converts .net/Visual Studio XML files to Markdown files for better project 
documentation.

#### Usage

Usage: xmldoc2md -xmlinfolder <folder for input> -mdoutfolder <folder for output>

    xmlinfolder: Path to folder with XML generated files to compile in.
    xmlinfile: Path to XML generated file to compile in.
    mdpagetemplate: Path to a markdown page template. This token: [content] will be replaced with generated content.
    mdoutfolder: Path to export compiled markdown file.

Note: you can use multiple xmlinfolder/xmlinfile parameters, as you wish.
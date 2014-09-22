/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using Bendyline.Base;
using Bendyline.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace xmldoc2md
{
    public static class Program
    {
        static void Main(string[] args)
        {
            CommandLineLogger cll = new CommandLineLogger();
            cll.Initialize();

            String xmlInputFolder = null;
            String xmlInputFile = null;
            String mdOutputFolder = null;
            String pageTemplatePath  = null;
            String pageTemplate = null;

            ProjectSpace ps = new ProjectSpace();

            for (int i=0; i<args.Length; i++)
            {
                String arg = args[i];
                String argCanon = arg.ToLowerCase();

                if (argCanon.StartsWith("/"))
                {
                    argCanon = "-" + argCanon.Substring(1);
                }

                switch (argCanon)
                {
                    case "-?":
                    case "-h":
                    case "-help":
                        OutputUsage();
                        break;

                    case "-mdpt":
                    case "-mdpagetemplate":
                        if (i < args.Length - 1)
                        {
                            i++;
                            pageTemplatePath = args[i];

                            if (pageTemplatePath != null && File.Exists(pageTemplatePath))
                            {
                                pageTemplate = FileUtilities.GetTextFromFile(pageTemplatePath);
                            }
                        }
                        break;

                    case "-xifo":
                    case "-xmlinfolder":
                        if (i < args.Length - 1)
                        {
                            i++;
                            xmlInputFolder = args[i];

                            if (xmlInputFolder != null && !Directory.Exists(xmlInputFolder))
                            {
                                Console.WriteLine("Could not find the input folder {0}", xmlInputFolder);

                                return;
                            }
                            
                            if (xmlInputFolder != null)
                            {
                                ps.ImportFromXmlDocFolder(xmlInputFolder);
                            }
                        }

                        break;

                    case "-xifi":
                    case "-xmlinfile":
                        if (i < args.Length - 1)
                        {
                            i++;
                            xmlInputFile = args[i];
                        }
                        

                        if (xmlInputFile != null && !File.Exists(xmlInputFile))
                        {
                            Console.WriteLine("Could not find the input file {0}", xmlInputFile);

                            return;
                        }

                        if (xmlInputFile != null)
                        {
                            ps.ImportFromXmlDocFile(xmlInputFile);
                        }
                        break;

                    case "-mdofo":
                    case "-mdoutfolder":
                        if (i < args.Length - 1)
                        {
                            i++;
                            mdOutputFolder = args[i];
                        }
                        break;
                }
            }

            if (String.IsNullOrEmpty(xmlInputFolder) && String.IsNullOrEmpty(xmlInputFile))
            {
                Console.WriteLine("No xml input folder was specified.\r\n");

                OutputUsage();

                return;
            }



            if (String.IsNullOrEmpty(mdOutputFolder))
            {
                Console.WriteLine("No Markdown output folder was specified.\r\n");

                OutputUsage();

                return;
            }

            if (!Directory.Exists(mdOutputFolder))
            {
                Console.WriteLine("Could not find the output folder {0}", mdOutputFolder);

                return;
            }

            try
            {
                ps.ExportMarkdownFiles(mdOutputFolder, pageTemplate);
            }
            catch (Exception e)
            {
                Log.Error("An error ocurrred when running this tool: " + e.Message);
            }
        }

        private static void OutputUsage()
        {
            Console.WriteLine(@"Usage: xmldoc2md -xmlinfolder <folder for input> -mdoutfolder <folder for output>

    xmlinfolder: Path to folder with XML generated files to compile in.
    xmlinfile: Path to XML generated file to compile in.
    mdpagetemplate: Path to a markdown page template. This token: [content] will be replaced with generated content.
    mdoutfolder: Path to export compiled markdown file.");
        }
    }
}

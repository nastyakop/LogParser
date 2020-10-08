using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace LogParser
{
    public class CmdOptions
    {
        [Option('f', "path", Required = true, HelpText = "Input path to file or directory.")]
        public string InputPath { get; set; }

        [Option('s', "search", Required = true, HelpText = "Input search pattern.")]
        public string InputSearch { get; set; }

        public static string HelpInfo
        {
            get
            {
                return "use -f <path> to input path file or directory\n" +
                    "use -s <search pattern> to inputsearch pattern\n";
            }
        }
    }
}

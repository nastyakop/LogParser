using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogParser
{
    class Program
    {
        static void ReadArgs(string[] args, ref Dictionary<String,String> files, ref string search)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string sw = args[i];

                switch (sw)
                {
                    case "-f":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("Missing argument!");
                        }
                        else
                        {
                            {
                                FileAttributes attr = File.GetAttributes(args[i + 1]);
                                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                                {
                                    string dir = args[i + 1];
                                    files = Directory.EnumerateFiles(dir, "*.log")
                                        .ToDictionary(f => f, File.ReadAllText);
                                }
                                else
                                {
                                    files.Add(args[i + 1], File.ReadAllText(args[i + 1]));
                                }
                            }
                        }
                        break;
                    case "-s":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("Missing argument!");
                        }
                        else
                        {
                            search = args[i + 1];
                        }
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            Dictionary<String, String> files = new Dictionary<string, string>();
            string search = "";
            ReadArgs(args, ref files, ref search);

        }
    }
}

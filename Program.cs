using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogParser
{
    class Program
    {
        static void ReadArgs(string[] args, ref Dictionary<String, String> files, ref string search)
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
                                    files = Directory.EnumerateFiles(dir, "*") //все файлы директории
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

        static List<string> Search(Dictionary<String, String> files, string search)
        {
            List<string> result = new List<string>();

            Regex regExpr = new Regex(search, RegexOptions.IgnoreCase);

            foreach (var f in files)
            {
                string text = f.Value;
                string fileName = f.Key;
                List<string> rowsInFile = FormRows(text);

                MatchCollection matches = regExpr.Matches(text);
                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;
                    int position = groups[0].Index;

                    string rowValue = FormResult(text, position, rowsInFile);
                    result.Add(" " + fileName + " ::" + rowValue);
                }
            }
            return result;
        }
        /// <summary>
        /// Возвращает номер и содержимое строки с найденном паттерном
        /// </summary>
        static string FormResult(string text, int symbolsBeforeEntry, List<string> rows)
        {
            string frag = text.Remove(symbolsBeforeEntry);
            Regex regExpr = new Regex("(\\r\\n)");
            MatchCollection matches = regExpr.Matches(frag);

            int rowNumber = matches.Count;

            return " " + (rowNumber + 1) + " : " + rows[rowNumber]; ;
        }
        /// <summary>
        /// формирует список строк в файле
        /// </summary>
        static List<string> FormRows(string text)
        {
            Regex regExpr = new Regex("(\\r\\n)");
            List<string> rows = regExpr.Split(text).ToList();
            rows.RemoveAll(x => x.Contains("\r\n"));
            return rows;
        }

        static void Output(List<string> result)
        {
            List<string> output = result.Distinct().ToList();
            FileStream fstream = new FileStream(@"results.txt", FileMode.OpenOrCreate);
            foreach (var str in output)
            {
                
                byte[] buff = System.Text.Encoding.Default.GetBytes(str+"\n\r");
                fstream.Write(buff);

                Console.WriteLine(str);
            }
            fstream.Close();
        }

        static void Main(string[] args)
        {
            Dictionary<String, String> files = new Dictionary<string, string>();
            string search = "";
            ReadArgs(args, ref files, ref search);
            List<string> result = Search(files, search);
            Output(result);
        }
    }
}

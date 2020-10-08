using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace LogParser
{

    class Program
    {
        static void ReadArgs(string[] args, ref List<String> files, ref string search)
        {
            CmdOptions ops = new CmdOptions();
            Parser.Default.ParseArguments<CmdOptions>(args).WithParsed<CmdOptions>(opts => ops = opts);
            
            if(ops.InputPath!=null & ops.InputSearch != null)
            {
                string dirPath = ops.InputPath;
                search = ops.InputSearch;
                FileAttributes attr = File.GetAttributes(dirPath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string dir = dirPath;
                    // собираем имна файлов
                    files = Directory.EnumerateFiles(dir, "*").ToList();
                }
                else
                {
                    files.Add(dirPath);
                }
            }
        }


        static List<string> Search(List<String> files, string search)
        {
            List<string> result = new List<string>();

            Regex regExpr = new Regex(search, RegexOptions.IgnoreCase);

            foreach (var f in files)
            {
                string fileName = f;
                string text = File.ReadAllText(f);
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

                byte[] buff = System.Text.Encoding.Default.GetBytes(str + "\n\r");
                fstream.Write(buff);

                Console.WriteLine(str);
            }
            fstream.Close();
        }

        static void Main(string[] args)
        {
            List<String> files = new List<String>();
            string search = "";
            ReadArgs(args, ref files, ref search);
            List<string> result = Search(files, search);
            Output(result);
        }
    }
}

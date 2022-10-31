using System;
using System.IO;
using System.Text;

namespace CodeMigrationValidator.Helpers
{
    public static class Printer
    {
        private static StringBuilder sb = new StringBuilder();
        public static void Print(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();

            sb.AppendLine(msg);
        }

        public static void WriteOutputFile(string outputFilepath, int cmtNumber)
        {
            using (StreamWriter w = File.AppendText($"{outputFilepath}/results_{cmtNumber}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt"))
            {
                w.Write(sb.ToString());
            }
        }
    }
}

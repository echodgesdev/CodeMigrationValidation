using CodeMigrationValidator.DataContracts;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using CodeMigrationValidator.Helpers;
using CodeMigrationValidator.DataConnectors;
using CodeMigrationValidator.Validators;

namespace CodeMigrationValidator
{
    public class Program
    {
        private static AppSettings _appSettings;
        public static AppSettings AppSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = new AppSettings();

                    var dir = $"{AssemblyDirectory}/AppSettings.json";
                    var textSettings = File.ReadAllText(dir);
                    _appSettings = JsonSerializer.Deserialize<AppSettings>(textSettings);
                }
                return _appSettings;
            }
        }
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static void Main(string[] args)
        {
            Printer.Print($"================== \n" +
                          $"CMT Validator 1.0  \n" +
                          $"================== \n");

            try
            {
                Printer.Print($"------------------------------------ \n" +
                              $"Gathering Actual CMT Contents....    \n" +
                              $"------------------------------------ \n",
                              ConsoleColor.DarkMagenta);

                var actualCMT    = new ActualCMTCrawler().Init(AppSettings)
                                                         .GetCodeMigration();


                AppSettings.SetActualCMTProperties(actualCMT);

                Printer.Print($"Done.", ConsoleColor.Magenta);

                Printer.Print($"------------------------------------ \n" +
                              $"Gathering Expected CMT Contents....  \n" +
                              $"------------------------------------ \n",
                              ConsoleColor.DarkMagenta);


                var expectedCMT = new ExpectedCMTCrawler().Init(AppSettings)
                                                           .GetCodeMigration();

                Printer.Print($"Done.", ConsoleColor.Magenta);

                CompareCMTs(actualCMT, expectedCMT);

                ValidateScriptsInCMT(actualCMT);
            }
            catch (Exception ex)
            {
                Printer.Print(ex.ToString(), ConsoleColor.Red);
            }

            Printer.Print("Comparison Complete. Press any key to exit.");

            Printer.WriteOutputFile(AssemblyDirectory, AppSettings.CMTNumber);

            Console.ReadKey();
        }
        private static void CompareCMTs(FoundationCodeMigration actualCMT, FoundationCodeMigration excpectedCMT)
        {
            var comparer = new CMTValidator();

            Printer.Print($"------------------------------------------- \n" +
                          $"Comparing Actual CMT with Expected CMT....  \n" +
                          $"------------------------------------------- \n",
                          ConsoleColor.DarkMagenta);

            Printer.Print($"---------------Comparing WIs---------------  \n",
                          ConsoleColor.Magenta);

            comparer.CompareWorkItems(excpectedCMT.WorkItems, actualCMT.WorkItems);

            Printer.Print($"Done.", ConsoleColor.Magenta);

            Printer.Print($"------------Comparing Contents-------------  \n",
                  ConsoleColor.Magenta);

            comparer.CompareContents(excpectedCMT.Contents, actualCMT.Contents);

            Printer.Print($"Done.", ConsoleColor.Magenta);
        }        

        private static void ValidateScriptsInCMT(FoundationCodeMigration cmt)
        {
            Printer.Print($"------------------------------------------- \n" +
                          $"Validating CMT Scripts....  \n" +
                          $"------------------------------------------- \n",
                          ConsoleColor.DarkMagenta);

            new ScriptValidator().Init(AppSettings)
                                 .Execute(cmt);

            Printer.Print($"Done.", ConsoleColor.Magenta);
        }
    }
}

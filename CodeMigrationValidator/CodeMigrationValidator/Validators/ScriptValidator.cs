using CodeMigrationValidator.DataContracts;
using System;
using System.IO;
using CodeMigrationValidator.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace CodeMigrationValidator.Validators
{
    /// <summary>
    /// The script validator validates all "scripts" found within a CodeMigration.
    /// Scripts include items such as SQL and Mongo (js) scripts where the types of validation performed on each file type are specified in the AppSettings.ScriptValidations section.
    /// This class was designed to formalize how developers write SQL scripts so their deployment can be traceable.
    /// </summary>
    public class ScriptValidator
    {
        private AppSettings _appSettings;

        public ScriptValidator Init(AppSettings appSettings)
        {
            _appSettings = appSettings;

            return this;
        }
        public void Execute(FoundationCodeMigration cmt)
        {
            foreach (var validation in _appSettings.ScriptValidations)
            {
                Printer.Print($"---------------Validating {validation.ContentTypeId}---------------  \n",
                                ConsoleColor.Magenta);

                var cmtItems = cmt.Contents.Where(c => c.ContentTypeId == validation.ContentTypeId)
                                           .ToList();


                Validate(cmtItems, validation);

                Printer.Print($"Done.", ConsoleColor.Magenta);
            }
        }

        private void Validate(List<CMTItem> items, ScriptValidation validation)
        {
            var fail = false;
            foreach (var sourceLocation in items.Select(i => i.SourceLocation))
            {
                //1. Validate folder has scripts of the correct type in them (e.g. *.sql files)

                var workItemNumber = sourceLocation.Split("//").Last();

                var scriptsInDirectory = Directory.EnumerateFiles(sourceLocation, $"*.{validation.ExtensionType}", SearchOption.AllDirectories);

                if (scriptsInDirectory.Count() <= 0)
                {
                    Printer.Print($"Directory {sourceLocation} is marked as {validation.ContentTypeId} and is expected to contain {validation.ExtensionType} files but does not.", ConsoleColor.Red);
                    continue;
                }

                //2. Validate that the scripts follow the patterns described in AppSettings.ScriptValidations

                foreach (var scriptDir in scriptsInDirectory)
                {
                    var success = ValidatePatterns(scriptDir, validation.ValidationPatterns, workItemNumber);

                    if (success == false)
                        fail = true;
                }
            }

            if(fail == false)
            {
                Printer.Print($"Scripts of type {validation.ContentTypeId} all passed validation.", ConsoleColor.Green);
            }
        }

        private bool ValidatePatterns(string scriptDir, List<ValidationPattern> validationPatterns, string workItemNumber)
        {
            var success = true;

            var scriptContents = File.ReadAllText(scriptDir);

            foreach(var pattern in validationPatterns)
            {
                var patt = pattern.Pattern.Replace("{workItemNumber}", workItemNumber);

                if(scriptContents.ToLowerInvariant().Contains(patt.ToLowerInvariant()) == false)
                {
                    Printer.Print($"Error validating {scriptDir}. \n" +
                                  $"Validation Error: {pattern.ValidationErrorMessage}", ConsoleColor.Red);

                    success = false;
                }
            }

            return success;
        }
    }
}

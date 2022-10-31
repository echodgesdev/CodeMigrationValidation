using CodeMigrationValidator.DataContracts;
using CodeMigrationValidator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMigrationValidator.Validators
{
    /// <summary>
    /// This class defines ways to compare the contents of the CM and ways to display when there are differences. 
    /// </summary>
    public class CMTValidator
    {
        public void CompareWorkItems(List<WorkItem> expectedWorkItems, List<WorkItem> actualWorkItems)
        {
            var fail = false;

            //1. Print out the WI expected in the CM and the items not expected in the CM

            Printer.Print($"Expected Work Items of CMT: \n" +
                  $"{WorkItemListToString(expectedWorkItems)}");

            Printer.Print($"Actual Work Items of CMT: \n" +
                  $"{WorkItemListToString(actualWorkItems)}");

            //2. Find items in Expected that are not in Actual

            var expectedOnly = expectedWorkItems.Where(e => actualWorkItems.Exists(a => a.Equals(e)) == false)
                                                .ToList();

            if (expectedOnly.Count() > 0)
            {
                var msg = $"The expected WI List does not match the actual WI List of the CM. \n" +
                          $"These items are only in the expected CM. \n" +
                          $"{WorkItemListToString(expectedOnly)}";

                Printer.Print(msg, ConsoleColor.Red);

                fail = true;
            }

            //3. Find items in Actual that are not in Expected

            var actualOnly = actualWorkItems.Where(a => expectedWorkItems.Exists(e => e.Equals(a)) == false)
                                           .ToList();

            if (actualOnly.Count() > 0)
            {
                var msg = $"The actual WI List does not match the expected WI List of the CM. " +
                          $"These items are only in the actual CM. \n" +
                          $"{WorkItemListToString(actualOnly)}";

                Printer.Print(msg, ConsoleColor.Yellow);

                fail = true;
            }

            if (fail == false)
            {
                Printer.Print($"The expected list of feature numbers matches the actual feature numbers :) ", ConsoleColor.Green);
            }
        }
        public void CompareContents(List<CMTItem> expectedContents, List<CMTItem> actualContents)
        {
            var fail = false;
            //1. Print out the contents expected in the CM and the items not expected in the CM

            Printer.Print($"Expected Contents of CMT: \n" +
                  $"{CMTItemListToString(expectedContents)}");

            Printer.Print($"Actual Contents of CMT: \n" +
                  $"{CMTItemListToString(actualContents)}");

            //2. Find items in Expected that are not in Actual

            var expectedOnly = expectedContents.Where(e => actualContents.Exists(a => a.Equals(e)) == false)
                                               .ToList();

            if (expectedOnly.Count() > 0)
            {
                var msg = $"The expected contents does not match the actual contents of the CM \n" +
                          $"These items are only in the expected CM. \n" +
                          $"{CMTItemListToString(expectedOnly)}";

                Printer.Print(msg, ConsoleColor.Red);

                fail = true;
            }

            //3. Find items in Actual that are not in Expected

            var actualOnly = actualContents.Where(a => expectedContents.Exists(e => e.Equals(a)) == false)
                                           .ToList();

            if (actualOnly.Count() > 0)
            {
                var msg = $"The actual contents does not match the expected contents of the CM. \n" +
                          $"These items are only in the actual CM. \n" +
                          $"Note: Some of these items may be part of the Log Mod Efforts. " +
                          $"Please confirm that they should exist in the CM. \n" +
                          $"{CMTItemListToString(actualOnly)}";

                Printer.Print(msg, ConsoleColor.Yellow);

                fail = true;
            }

            if (fail == false)
            {
                Printer.Print($"The expected contents matches the actual contents :) ", ConsoleColor.Green);
            }
        }

        private string WorkItemListToString(List<WorkItem> workItems)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Work Item Number");
            foreach (var wi in workItems)
            {
                sb.AppendLine($"{wi.Id}");
            }

            return sb.ToString();
        }
        private string CMTItemListToString(List<CMTItem> cmItems)
        {
            var colPadding = 30;
            var sb = new StringBuilder();

            sb.AppendLine($"{"ContentTypeId".PadRight(colPadding)}, {"Source Location".PadRight(colPadding)}");

            foreach (var item in cmItems)
            {
                sb.AppendLine($"{item.ContentTypeId.PadRight(colPadding)}, {item.SourceLocation.PadRight(colPadding)}");
            }

            return sb.ToString();
        }
    }
}

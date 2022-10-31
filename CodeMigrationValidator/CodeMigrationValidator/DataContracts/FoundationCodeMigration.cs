using System;
using System.Collections.Generic;

namespace CodeMigrationValidator.DataContracts
{
    public class FoundationCodeMigration
    {
        public List<CMTItem> Contents { get; set; }
        public List<WorkItem> WorkItems { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string QAEnvironmentId {get;set;}
    }
}

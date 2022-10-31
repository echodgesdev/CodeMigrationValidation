using System;

namespace CodeMigrationValidator.DataContracts
{
    public class WorkItem : IEquatable<WorkItem>
    {
        public int Id { get; set; }

        public bool Equals(WorkItem other)
        {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }
    }
}

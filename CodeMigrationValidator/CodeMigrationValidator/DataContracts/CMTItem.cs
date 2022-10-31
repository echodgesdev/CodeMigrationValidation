using System;
using System.Collections.Generic;

namespace CodeMigrationValidator.DataContracts
{
    public class CMTItem: IEquatable<CMTItem>
    {
        public string ContentTypeId { get; set; }
        public string SourceLocation { get; set; }

        public bool Equals(CMTItem other)
        {
            if (other is null)
                return false;

            return (this.ContentTypeId == other.ContentTypeId && this.SourceLocation.ToLowerInvariant() == other.SourceLocation.ToLowerInvariant());
        }
    }
}

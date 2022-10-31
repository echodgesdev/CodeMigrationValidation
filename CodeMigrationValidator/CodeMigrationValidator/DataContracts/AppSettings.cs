using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeMigrationValidator.DataContracts
{
    public class AppSettings
    {
        //Things that may change often
        public int CMTNumber { get; set; }
        public List<int> FeatureNumbers { get; set; }

        //Things that change Less Often
        public List<string> DesktopClientBuildProjectInclusions { get; set; }
        public List<string> ServerBuildProjectInclusions { get; set; }

        public List<ScriptValidation> ScriptValidations { get; set; }

        //Things that rarely Change
        public string CodeBuildBaseDirectory { get; set; }
        public string SQLScriptsBaseDirectory { get; set; }
        public string MongoScriptsBaseDirectory { get; set; }
        public string FoundationBaseUrl { get; set; }

        #region Items Set from Actual CMT
        [JsonIgnore]
        public DateTime CodeMigrationBeginDate { get; private set; }

        [JsonIgnore]
        public DateTime CodeMigrationEndDate { get; private set; }

        [JsonIgnore]
        public string QAEnvironmentId { get; private set; }
        #endregion

        /// <summary>
        /// Some App settings originate in the CM itself and are needed to accurately calculate the expected CM
        /// </summary>
        /// <param name="actualCMT"></param>
        public void SetActualCMTProperties(FoundationCodeMigration actualCMT)
        {
            QAEnvironmentId = actualCMT.QAEnvironmentId;
            CodeMigrationBeginDate = actualCMT.CreatedDate;

            //Target Date might be default if the CM hasn't been migrated to prodcution. This is the most likely scenario.
            CodeMigrationEndDate = actualCMT.TargetDate != new DateTime() ? actualCMT.TargetDate : CodeMigrationBeginDate.AddDays(100);
        }
    }

    public class ScriptValidation
    {
        public string ExtensionType { get; set; }

        public string ContentTypeId { get; set; }

        public List<ValidationPattern> ValidationPatterns { get; set; }
    }

    public class ValidationPattern
    {
        public string Pattern { get; set; }
        public string ValidationErrorMessage { get; set; }
    }
}

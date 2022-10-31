using System.Collections.Generic;

namespace CodeMigrationValidator
{
    public static class Constants
    {
        public static string CONTENT_TYPE_ID_WEB_SERVICE    = "WEB-SERVICE";
        public static string CONTENT_TYPE_ID_APPLICATION    = "APPLICATION";
        public static string CONTENT_TYPE_ID_DATABASE       = "DATABASE";
        public static string CONTENT_TYPE_ID_CLOUD_DATABASE = "CLOUD-DATABASE";

        //Key - name of environment that the code is deployed to; Value - base directory path
        public static Dictionary<string, string> QA_ENVIRONMENT_BUILD_FOLDERS = new Dictionary<string, string>()
        {
            { "CRQA", "QA" },
            { "POST", "PROD" }
        };
    }
}

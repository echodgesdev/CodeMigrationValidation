using CodeMigrationValidator.DataContracts;

namespace CodeMigrationValidator.DataConnectors
{
    public interface ICMTCrawler
    {
        FoundationCodeMigration GetCodeMigration();
        ICMTCrawler Init(AppSettings appSettings);
    }
}
using CodeMigrationValidator.DataContracts;
using System.Collections.Generic;
using Flurl;
using Flurl.Http;
using CodeMigrationValidator.Extensions;
using System;

namespace CodeMigrationValidator.DataConnectors
{
    /// <summary>
    /// This class crawls through Foundation APIs to build a snapshot of what is acutally in the CMT.
    /// </summary>
    public class ActualCMTCrawler : ICMTCrawler
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AppSettings _appSettings;

        public ICMTCrawler Init(AppSettings appSettings)
        {
            _appSettings = appSettings;
            FlurlConfiguration.ConfigureDomainForDefaultCredentials(_appSettings.FoundationBaseUrl);

            return this;
        }

        public FoundationCodeMigration GetCodeMigration()
        {
            var result = new FoundationCodeMigration();

            var url = _appSettings.FoundationBaseUrl
                                  .AppendPathSegment($"api/requests/codemigrations/{_appSettings.CMTNumber}");
            try
            {
                result = url.GetJsonAsync<FoundationCodeMigration>().Result;
            }
            catch(Exception ex)
            {
                _logger.Error($"An error occured while calling {url}", ex);

                throw;
            }

            return result;
        }
    }
}

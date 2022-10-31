using Flurl.Http;
using Flurl.Http.Configuration;
using System.Net.Http;

namespace CodeMigrationValidator.Extensions
{
    public class UseDefaultCredentialsClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler { UseDefaultCredentials = true };
        }
    }

    public static class FlurlConfiguration
    {
        public static void ConfigureDomainForDefaultCredentials(string url)
        {
            FlurlHttp.ConfigureClient(url, cli =>
                cli.Settings.HttpClientFactory = new UseDefaultCredentialsClientFactory());
        }
    }
}

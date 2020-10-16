using System.Threading;
using AzureFunctions.SqlBinding;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace FunctionApp
{
    internal class SqlBindingTokenProvider : ISqlBindingTokenProvider
    {
        public async Task<string> GetToken(CancellationToken cancellationToken)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/", null, cancellationToken);
            return accessToken;
        }
    }
}
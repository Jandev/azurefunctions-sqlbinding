using AzureFunctions.SqlBinding;
using FunctionApp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // If you want to use an access token in the connection, register an implemenation for the `AzureFunctions.SqlBinding.ISqlBindingTokenProvider`
            builder.Services.AddTransient<ISqlBindingTokenProvider, SqlBindingTokenProvider>();
        }
    }
}
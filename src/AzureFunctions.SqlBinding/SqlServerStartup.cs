using AzureFunctions.SqlBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(SqlServerStartup))]
namespace AzureFunctions.SqlBinding
{
	public class SqlServerStartup : IWebJobsStartup
	{
		public void Configure(IWebJobsBuilder builder)
		{
			builder.AddSqlServerBinding();
		}
	}
}

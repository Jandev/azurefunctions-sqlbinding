using Microsoft.Azure.WebJobs.Description;
using System;

namespace AzureFunctions.SqlBinding
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	[Binding]
	public class SqlServerAttribute : Attribute
	{
		[ConnectionString(Default = "SqlServerConnectionString")]
		public string ConnectionString { get; set; }

		[AutoResolve]
		public string Query { get; set; }

	}
}

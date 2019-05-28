using System;
using Microsoft.Azure.WebJobs;

namespace AzureFunctions.SqlBinding
{
	public static class SqlServerExtension
	{
		public static IWebJobsBuilder AddSqlServerBinding(this IWebJobsBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			builder.AddExtension<SqlServerBinding>();
			return builder;
		}
	}
}

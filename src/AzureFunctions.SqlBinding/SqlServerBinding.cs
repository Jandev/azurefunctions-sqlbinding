﻿using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFunctions.SqlBinding
{
	[Extension(nameof(SqlServerBinding))]
	public class SqlServerBinding : IExtensionConfigProvider
	{
		private static ISqlBindingTokenProvider tokenProvider ;		
		public SqlServerBinding(IServiceProvider services)
		{
			
			tokenProvider = services.GetRequiredService<ISqlBindingTokenProvider>();
		}

		public void Initialize(ExtensionConfigContext context)
		{
			var rule = context.AddBindingRule<SqlServerAttribute>();

			rule.BindToInput<SqlServerModel>(BuildFromAttribute);
			rule.BindToInput<IEnumerable<SqlServerModel>>(BuildCollectionFromAttribute);
		}

		private async Task<IEnumerable<SqlServerModel>> BuildCollectionFromAttribute(
			SqlServerAttribute attribute,
			CancellationToken cancellationToken)
		{
			var collection = new List<SqlServerModel>();
			using (var connection = new SqlConnection(attribute.ConnectionString))
			{
				if (tokenProvider != null)
				{
					connection.AccessToken = await tokenProvider.GetToken(cancellationToken);
				}

				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.Connection = connection;
					command.CommandText = attribute.Query;

					var reader = await command.ExecuteReaderAsync(cancellationToken);
					var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
					while (await reader.ReadAsync(cancellationToken))
					{
						var expando = new ExpandoObject() as IDictionary<string, object>;
						foreach (var name in names) expando[name] = reader[name];
						collection.Add(
							new SqlServerModel
							{
								Record = expando
							});
					}
				}
			}

			return collection;
		}

		private async Task<SqlServerModel> BuildFromAttribute(
			SqlServerAttribute attribute,
			CancellationToken cancellationToken)
		{
			using (var connection = new SqlConnection(attribute.ConnectionString))
			{
				if (tokenProvider != null)
				{
					connection.AccessToken = await tokenProvider.GetToken(cancellationToken);
				}
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.Connection = connection;
					command.CommandText = attribute.Query;

					var reader = await command.ExecuteReaderAsync(cancellationToken);
					var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
					while (await reader.ReadAsync(cancellationToken))
					{
						var expando = new ExpandoObject() as IDictionary<string, object>;
						foreach (var name in names) expando[name] = reader[name];
						return new SqlServerModel
						{
							Record = expando
						};
					}

					return new SqlServerModel();
				}
			}
		}
	}
	
	
}

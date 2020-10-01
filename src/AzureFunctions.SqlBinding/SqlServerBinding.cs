using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
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
        private string accessToken = null;
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
                if (isManagedIdentity(attribute.ConnectionString))
                {
                    accessToken = await GetAccessToken();
                    connection.AccessToken = accessToken;
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
                if (isManagedIdentity(attribute.ConnectionString))
                {
                    accessToken = await GetAccessToken();
                    connection.AccessToken = accessToken;
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

        private async Task<string> GetAccessToken()
        {
            var tokenProvider = new AzureServiceTokenProvider();
            string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
            return accessToken;

        }

        private bool isManagedIdentity(string connectionString)
        {
            var terms = new[] { "UserID", "Password", "PWD=", "UID=" };

            foreach (var term in terms)
            {
                if (connectionString.IndexOf(term, 0, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    return false;
                }
            }
            return true;
        }



    }
}

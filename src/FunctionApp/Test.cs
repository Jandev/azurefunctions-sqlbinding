using AzureFunctions.SqlBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApp
{
	public static class Test
	{
		[FunctionName(nameof(GetSingleRecord))]
		public static IActionResult GetSingleRecord(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(GetSingleRecord))]
			HttpRequest req,
			[SqlServer(Query = "SELECT TOP 1 Id, Name, Description FROM MyTable")]
			SqlServerModel model)
		{
			return new OkObjectResult(model.Record.Id);
		}

		[FunctionName(nameof(GetCollectionFromSqlServer))]
		public static IActionResult GetCollectionFromSqlServer(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(GetCollectionFromSqlServer))]
			HttpRequest req,
			[SqlServer(Query = "SELECT TOP 100 Id, Name, Description FROM MyTable")]
			IEnumerable<SqlServerModel> collection)
		{
			return new OkObjectResult(collection.Select(m => m.Record.Id));
		}
	}
}

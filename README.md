# Overview

This project is created to provide a simple 
Microsoft SQL Server binding for Azure Functions.

# Supported features

The following commands are supported as of now.

## Input binding

The input binding can be used as like the following
piece of code.

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

In order for this to work you need a `local.settings.json`
file with a proper connectionstring. If not specified
the default name `SqlServerConnectionString` will be used.

	{
		"IsEncrypted": false,
		"Values": {
			"AzureWebJobsStorage": "UseDevelopmentStorage=true",
			"FUNCTIONS_WORKER_RUNTIME": "dotnet"
		},
		"ConnectionStrings": {
			"SqlServerConnectionString": "Server=(localdb)\\snelstart;Database=MySqlBindingDatabase;Trusted_Connection=True",
			"providerName": "System.Data.SqlClient"
		}
	}
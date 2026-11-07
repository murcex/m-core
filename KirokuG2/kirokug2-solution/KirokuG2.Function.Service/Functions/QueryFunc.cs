namespace KirokuG2.Service.Functions
{
	using KirokuG2.Service.Core;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Azure.Functions.Worker;
	using Microsoft.Extensions.Logging;
	using PlyQor.Client;
	using System.Threading.Tasks;

	public class QueryFunc
	{
		private readonly ILogger<QueryFunc> logger;

		public QueryFunc(ILogger<QueryFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Query")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			FunctionContext functionContext)
		{
			string token = req.Query["token"];
			string id = req.Query["id"];

			string responseMessage = "";

			if (string.IsNullOrEmpty(token))
			{
				return new OkObjectResult(responseMessage);
			}

			if (string.IsNullOrEmpty(id))
			{
				return new OkObjectResult(responseMessage);
			}

			if (!Configuration.QueryTokens.Contains(token))
			{
				responseMessage = "400";
				return new OkObjectResult(responseMessage);
			}

			var result = Configuration.Storage.Select(id.ToUpper());

			if (result.GetPlyStatus())
			{
				var data = result.GetPlyData();

				if (string.IsNullOrEmpty(data))
				{
					return new OkObjectResult("404");
				}

				return new OkObjectResult(data);
			}
			else
			{
				return new OkObjectResult("404");
			}
		}
	}
}

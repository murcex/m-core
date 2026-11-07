namespace KirokuG2.Service.Functions
{
	using KirokuG2.Service.Core;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Azure.Functions.Worker;
	using Microsoft.Extensions.Logging;
	using System;
	using System.IO;
	using System.Threading.Tasks;

	public class CollectorFunc
	{
		private readonly ILogger<CollectorFunc> logger;

		public CollectorFunc(ILogger<CollectorFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Collector")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			FunctionContext functionContext)
		{
			string responseMessage = string.Empty;

			string token = req.Query["token"];

			if (string.IsNullOrEmpty(token))
			{
				return new OkObjectResult(responseMessage);
			}

			if (!Configuration.CollectorTokens.Contains(token))
			{
				responseMessage = "400";
				return new OkObjectResult(responseMessage);
			}

			// send requestBody to PlyQor storage
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			if (string.IsNullOrEmpty(requestBody))
			{
				responseMessage = "404.3";
				return new OkObjectResult(responseMessage);
			}

			Configuration.Storage.Insert(Guid.NewGuid().ToString(), requestBody, "upload");

			responseMessage = "200";
			return new OkObjectResult(responseMessage);
		}
	}
}

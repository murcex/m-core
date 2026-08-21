using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Murcex.Vyudro.Internal.Audit;
using System;

namespace Murcex.Vyudro.Function.Audit.Functions
{
	public class PortalFunc
	{
		private readonly ILogger<PortalFunc> logger;

		public PortalFunc(ILogger<PortalFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Portal")]
		public IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			FunctionContext functionContext)
		{
			using (var klog = KManager.NewInstance("Test-Portal"))
			{
				try
				{
					return PortalManager.GetPage(req, klog);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);

					throw;
				}
			}
		}
	}
}

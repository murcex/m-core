using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Murcex.Vyudro.Internal.Audit;
using System;

namespace Murcex.Vyudro.Function.Audit.Functions
{
	public static class PortalFunc
	{
		[FunctionName("Portal")]
		public static IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
		ILogger log)
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

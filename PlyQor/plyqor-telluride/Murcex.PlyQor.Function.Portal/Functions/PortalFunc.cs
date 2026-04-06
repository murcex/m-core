using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Murcex.PlyQor.Internal.Portal;
using System;

namespace Murcex.PlyQor.Function.Portal.Functions
{
	public static class PortalFunc
	{
		[FunctionName("Portal")]
		public static IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
		ILogger log)
		{
			using (var klog = KManager.NewInstance("PlyQor-Portal"))
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

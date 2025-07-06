using KirokuG2.Internal.Portal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KirokuG2.Function.Portal.Functions
{
	public static class PortalFunc
	{
		[FunctionName("Portal")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			using (var klog = KManager.NewInstance("Kiroku-Portal"))
			{
				try
				{
					return PortalManager.GetPage(req, klog);
				}
				catch (Exception ex)
				{
					klog.Error($"Function Level Exception: {ex}");

					throw;
				}
			}
		}
	}
}

using KirokuG2.Internal.Portal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KirokuG2.Function.Portal.Functions
{
	public class PortalFunc
	{
		private readonly ILogger<PortalFunc> logger;

		public PortalFunc(ILogger<PortalFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Portal")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			FunctionContext functionContext)
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

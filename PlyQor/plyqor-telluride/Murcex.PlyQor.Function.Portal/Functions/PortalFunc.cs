using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Murcex.PlyQor.Internal.Portal;
using System;

namespace Murcex.PlyQor.Function.Portal.Functions
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
			FunctionContext context)
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

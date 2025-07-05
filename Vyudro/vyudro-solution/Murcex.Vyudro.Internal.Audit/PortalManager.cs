using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Internal.Audit.Core;

namespace Murcex.Vyudro.Internal.Audit
{
	public class PortalManager
	{
		public static void Initialize(Dictionary<string, Dictionary<string, string>> cfg)
		{
			Initializer.Execute(cfg);
		}

		public static IActionResult GetPage(HttpRequest request, IKLog klog)
		{
			return Configuration.SiteManager.GeneratePage(request, klog);
		}
	}
}

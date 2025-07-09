using KirokuG2.Internal.Portal.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KirokuG2.Internal.Portal
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

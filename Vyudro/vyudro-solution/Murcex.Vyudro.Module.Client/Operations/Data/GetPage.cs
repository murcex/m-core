using KirokuG2;
using Microsoft.AspNetCore.Http;
using Murcex.Vyudro.Module.Client.Core;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Module.Client.Operations.Data
{
	public class GetPage
	{
		public static bool Execute(Configuration configuration, HttpRequest request, IKLog klog, Dictionary<string, string> aux, out Page page, out string message)
		{
			var pageName = request.Query["page"];

			if (string.IsNullOrEmpty(pageName))
			{
				page = null;
				message = "Page Name Not Found in Request";
				return false;
			}

			if (configuration.PageCache.TryGetValue(pageName, out page))
			{
				message = $"Generating Page {pageName}";
				klog.Trace(message);
				aux.Add("Page", page.Name);
				return true;
			}
			else
			{
				message = $"Page {pageName} Not Found";
				return false;
			}
		}
	}
}

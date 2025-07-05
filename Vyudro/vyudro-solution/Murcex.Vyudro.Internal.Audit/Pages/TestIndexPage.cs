using Murcex.Vyudro.Internal.Utilities.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class TestIndexPage
	{
		public static Dictionary<string, string> ExecuteIndex(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Index Page Func");

			var dynamicElements = new Dictionary<string, string>();

			var hostname = pageFuncData.Elements.GetValue("WEBSITE_HOSTNAME");

			var token = pageFuncData.Auxiliary.GetValue("Token");

			var link = $"{hostname}/api/Portal?page=test-static&session-token={token}";

			dynamicElements.Add("dym-static-link", link);

			link = $"{hostname}/api/Portal?page=test-dynamic&session-token={token}";

			dynamicElements.Add("dym-dynamic-link", link);

			return dynamicElements;
		}
	}
}

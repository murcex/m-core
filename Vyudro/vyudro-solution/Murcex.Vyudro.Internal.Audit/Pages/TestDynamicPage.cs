using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class TestDynamicPage
	{
		public static Dictionary<string, string> ExecuteStatus(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Dynamic Page Func");

			var dynamicElements = new Dictionary<string, string>
			{
				{ "dym-token", pageFuncData.Auxiliary.GetValue("Token") },
				{ "dym-guid-value", Guid.NewGuid().ToString() }
			};

			return dynamicElements;
		}
	}
}

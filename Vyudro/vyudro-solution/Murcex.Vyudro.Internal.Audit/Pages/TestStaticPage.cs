using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class TestStaticPage
	{
		public static Dictionary<string, string> ExecuteStatus(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Static Page Func");

			var dynamicElements = new Dictionary<string, string>
			{
				{ "dym-token", pageFuncData.Auxiliary.GetValue("Token") }
			};

			return dynamicElements;
		}
	}
}

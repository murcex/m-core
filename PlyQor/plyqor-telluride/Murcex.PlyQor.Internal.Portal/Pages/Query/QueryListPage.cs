using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Query
{
	public class QueryListPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Query List Page Func");

			var dynamicElements = new Dictionary<string, string>();

			var token = pageFuncData.Auxiliary.GetValue("Token");

			var container = pageFuncData.Request.Query["container"];

			dynamicElements.Add("dym-token", token);
			dynamicElements.Add("dym-container", container);

			return dynamicElements;
		}
	}
}

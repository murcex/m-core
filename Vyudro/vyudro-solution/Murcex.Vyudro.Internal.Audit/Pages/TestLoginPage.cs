using Murcex.Vyudro.Internal.Utilities.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class TestLoginPage
	{
		public static Dictionary<string, string> ExecuteLogin(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Login Page Func");

			var dynamicElements = new Dictionary<string, string>
			{
				{ "dym-token", pageFuncData.Auxiliary.GetValue("Token") }
			};

			return dynamicElements;
		}
	}
}

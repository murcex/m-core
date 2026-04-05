using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Main
{
	public class LoginPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
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

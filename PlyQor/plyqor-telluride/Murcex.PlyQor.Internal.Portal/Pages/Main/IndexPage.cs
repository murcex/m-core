using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Main
{
	internal class IndexPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Index Page Func");

			var dynamicElements = new Dictionary<string, string>
			{
				{ "dym-token", pageFuncData.Auxiliary.GetValue("Token") }
			};

			return dynamicElements;
		}
	}
}

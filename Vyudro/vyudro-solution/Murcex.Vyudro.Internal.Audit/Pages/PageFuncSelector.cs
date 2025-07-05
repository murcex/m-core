using Murcex.Vyudro.Internal.Utilities.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class PageFuncSelector
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			var page = pageFuncData.Auxiliary.GetValue("Page");

			if (string.IsNullOrEmpty(page))
			{
				pageFuncData.KLog.Error("Page name not found");
				throw new Exception("Page name not found");
			}

			switch (page)
			{
				case "test-signin":
					return new Dictionary<string, string>();
				case "test-login":
					return TestLoginPage.ExecuteLogin(pageFuncData);
				case "test-index":
					return TestIndexPage.ExecuteIndex(pageFuncData);
				case "test-static":
					return TestStaticPage.ExecuteStatus(pageFuncData);
				case "test-dynamic":
					return TestDynamicPage.ExecuteStatus(pageFuncData);
				default:
					pageFuncData.KLog.Error($"Page Function not found for {page}");
					throw new Exception($"Page Function not found for {page}");
			}
		}
	}
}

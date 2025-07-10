using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages
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
				case "signin":
					return new Dictionary<string, string>();
				case "login":
					return LoginPage.Execute(pageFuncData);
				case "containers":
					return ContainersPage.Execute(pageFuncData);
				case "container":
					return ContainerPage.Execute(pageFuncData);

				default:
					pageFuncData.KLog.Error($"Page Function not found for {page}");
					throw new Exception($"Page Function not found for {page}");
			}
		}
	}
}

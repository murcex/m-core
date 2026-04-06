using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Portal.Pages.Container;
using Murcex.PlyQor.Internal.Portal.Pages.Main;
using Murcex.PlyQor.Internal.Portal.Pages.Query;
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
				// main pages
				case "plyqor-signin":
					return new Dictionary<string, string>();
				case "plyqor-login":
					return LoginPage.Execute(pageFuncData);
				case "plyqor-index":
					return IndexPage.Execute(pageFuncData);

				// container pages
				case "container-list":
					return ContainerListPage.Execute(pageFuncData);
				case "container-create":
					return ContainerCreatePage.Execute(pageFuncData);
				case "container-select":
					return ContainerSelectPage.Execute(pageFuncData);
				case "container-delete":
					return ContainerDeletePage.Execute(pageFuncData);
				case "container-operation":
					return ContainerOperationPage.Execute(pageFuncData);

				// query pages
				case "query-list":
					return QueryListPage.Execute(pageFuncData);
				case "query-insert":
				case "query-select":
				case "query-select-list":
				case "query-select-tags":
				case "query-update":
				case "query-update-tag":
				case "query-delete":
					return QueryListPage.Execute(pageFuncData);
				case "query-operation":
					return QueryOperationPage.Execute(pageFuncData);

				default:
					pageFuncData.KLog.Error($"Page Function not found for {page}");
					throw new Exception($"Page Function not found for {page}");
			}
		}
	}
}

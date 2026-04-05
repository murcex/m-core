using Murcex.PlyQor.Internal.Portal.Pages.Query.QueryOperationInternal;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Query
{
	public class QueryOperationPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			// Convert the StringValues to a string to avoid CS9135 error
			var operation = pageFuncData.Request.Query["operation"].ToString();

			return operation switch
			{
				"insert" => QueryInsertPage.Execute(pageFuncData),
				"select" => QuerySelectPage.Execute(pageFuncData),
				"update" => QueryUpdatePage.Execute(pageFuncData),
				"delete" => QueryDeletePage.Execute(pageFuncData),
				"select-list" => QuerySelectListPage.Execute(pageFuncData),
				"select-tags" => QuerySelectTagsPage.Execute(pageFuncData),
				"update-tag" => QueryUpdateTagPage.Execute(pageFuncData),
				_ => throw new Exception($"Unknown operations: {operation}")
			};
		}
	}
}
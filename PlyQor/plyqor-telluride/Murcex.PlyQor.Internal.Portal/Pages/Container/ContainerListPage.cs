using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.Vyudro.Module.Client.Models;
using System.Text;

namespace Murcex.PlyQor.Internal.Portal.Pages.Container
{
	public class ContainerListPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing List Containers Page Func");

			var dynamicElements = new Dictionary<string, string>();

			var token = pageFuncData.Auxiliary.GetValue("Token");

			var operation = pageFuncData.Request.Query["operation"];

			var page = string.Empty;
			if (string.IsNullOrEmpty(operation))
			{
				pageFuncData.KLog.Error("Operation type is not provided in the request query.");
				throw new ArgumentException("Operation type is required in the request query.");
			}
			else
			{
				switch (operation)
				{
					case "select":
						page = "container-select";
						break;
					case "delete":
						page = "container-delete";
						break;
					case "query":
						page = "query-list";
						break;
					default:
						pageFuncData.KLog.Error($"Unknown operation type: {operation}");
						throw new ArgumentException($"Unknown operation type: {operation}");
				}
			}

			var containers = PlyQorContainerManager.ListContainers();

			StringBuilder sb = new();

			if (containers == null || containers.Count == 0)
			{
				pageFuncData.KLog.Trace("No containers found.");
			}
			else
			{
				foreach (var container in containers)
				{
					sb.AppendLine($"<option value=\"{container}\">{container}</option>");
				}
			}

			dynamicElements.Add("dym-container-list", sb.ToString());
			dynamicElements.Add("dym-token", token);
			dynamicElements.Add("dym-page", page);

			return dynamicElements;
		}
	}
}

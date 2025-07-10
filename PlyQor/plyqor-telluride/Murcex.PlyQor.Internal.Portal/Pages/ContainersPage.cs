using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.Vyudro.Module.Client.Models;
using System.Text;

namespace Murcex.PlyQor.Internal.Portal.Pages
{
	public class ContainersPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing List Containers Page Func");

			var dynamicElements = new Dictionary<string, string>();

			var hostname = pageFuncData.Elements.GetValue("WEBSITE_HOSTNAME");

			var token = pageFuncData.Auxiliary.GetValue("Token");

			var link = $"{hostname}/api/Portal?page=container&session-token={token}&container=";

			var containers = PlyQorContainerManager.ListContainers();

			StringBuilder sb = new StringBuilder();

			if (containers == null || containers.Count == 0)
			{
				pageFuncData.KLog.Trace("No containers found.");
				sb.AppendLine("// <li>No containers found.</li>");
			}
			else
			{
				foreach (var container in containers)
				{
					sb.AppendLine($"// <li><a href=\"{link}{container}\">{container}</a></li>");
				}
			}

			dynamicElements.Add("dym-container-list", sb.ToString());

			return dynamicElements;
		}
	}
}

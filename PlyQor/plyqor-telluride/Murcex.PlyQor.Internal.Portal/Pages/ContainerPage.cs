using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages
{
	public class ContainerPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Container Page Func");

			var dynamicElements = new Dictionary<string, string>();

			var hostname = pageFuncData.Elements.GetValue("WEBSITE_HOSTNAME");

			var token = pageFuncData.Auxiliary.GetValue("Token");

			var link = $"{hostname}/api/Portal?page=containers&session-token={token}";

			dynamicElements.Add("dym-list-containers-link", link);

			var containerName = pageFuncData.Request.Query["container"];

			if (string.IsNullOrEmpty(containerName))
			{
				pageFuncData.KLog.Error("Container name is not provided in the request query.");
				throw new ArgumentException("Container name is required in the request query.");
			}

			var container = PlyQorContainerManager.GetContainer(containerName);

			var dynamicContainerName = container.Name ?? throw new ArgumentException("Container name is not found in the container data.");
			var dynamicRetentionValue = container.Retention.ToString() ?? throw new ArgumentException("Container retention value is not found in the container data.");
			var dynamicPrimaryToken = container.PrimaryToken;
			var dynamicSecondaryToken = container.SecondaryToken;

			dynamicElements.Add("dynamic-container-name", dynamicContainerName);
			dynamicElements.Add("dynamic-retention-value", dynamicRetentionValue);
			dynamicElements.Add("dynamic-primary-token", dynamicPrimaryToken);
			dynamicElements.Add("dynamic-secondary-token", dynamicSecondaryToken);

			return dynamicElements;
		}
	}
}

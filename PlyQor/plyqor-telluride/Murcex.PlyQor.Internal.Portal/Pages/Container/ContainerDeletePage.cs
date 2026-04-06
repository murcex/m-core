using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.PlyQor.Internal.Portal.Pages.Container;

public class ContainerDeletePage
{
	public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
	{
		pageFuncData.KLog.Trace($"Executing DeleteContainer Page Func");

		var dynamicElements = new Dictionary<string, string>();

		var token = pageFuncData.Auxiliary.GetValue("Token");

		dynamicElements.Add("dym-token", token);

		// Safely retrieve the containerName from the query
		if (!pageFuncData.Request.Query.TryGetValue("container", out var containerName) || string.IsNullOrEmpty(containerName))
		{
			pageFuncData.KLog.Error("Container name is not provided in the request query.");
			throw new ArgumentException("Container name is required in the request query.");
		}

		// Fix for CS8601: Ensure containerName is not null
		var containerNameValue = containerName.ToString();
		if (string.IsNullOrEmpty(containerNameValue))
		{
			pageFuncData.KLog.Error("Container name is null or empty after conversion.");
			throw new ArgumentException("Container name is required and cannot be null or empty.");
		}

		dynamicElements.Add("dym-container", containerNameValue);

		return dynamicElements;
	}
}

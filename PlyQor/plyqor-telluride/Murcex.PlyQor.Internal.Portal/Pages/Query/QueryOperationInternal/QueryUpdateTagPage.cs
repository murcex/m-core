using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Container.Core;
using Murcex.Vyudro.Module.Client.Models;
using PlyQor.Client;
using System.Text.Json;

namespace Murcex.PlyQor.Internal.Portal.Pages.Query.QueryOperationInternal
{
	public class QueryUpdateTagPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			var dynamicElements = new Dictionary<string, string>();

			var token = pageFuncData.Auxiliary.GetValue("Token");

			dynamicElements.Add("dym-token", token);

			// get query params from pageFuncData
			// - tag
			// - new tag
			var tag = pageFuncData.Request.Query["plyqor-tag"];
			if (string.IsNullOrEmpty(tag))
			{
				pageFuncData.KLog.Error("Tag not found in query parameters");
				throw new Exception("Tag not found in query parameters");
			}

			var newTag = pageFuncData.Request.Query["plyqor-tag-new"];
			if (string.IsNullOrEmpty(newTag))
			{
				pageFuncData.KLog.Error("NewTag not found in query parameters");
				throw new Exception("NewTag not found in query parameters");
			}

			// get container name from pageFuncData
			var containerName = pageFuncData.Request.Query["container"];

			if (string.IsNullOrEmpty(containerName))
			{
				pageFuncData.KLog.Error("Container name not found in query parameters");
				throw new Exception("Container name not found in query parameters");
			}
			dynamicElements.Add("dym-container", containerName);

			// get token from ContainerManager
			var plyqorToken = PlyQorContainerManager.GetToken(containerName);

			if (string.IsNullOrEmpty(plyqorToken))
			{
				pageFuncData.KLog.Error($"Token not found for container {containerName}");
				throw new Exception($"Token not found for container {containerName}");
			}

			// build PlyQor Client: endpoint + container + token
			var plyQorClient = new PlyClient(Configuration.PlyQorEndpoint, containerName, plyqorToken);

			var queryResult = plyQorClient.UpdateTag(tag, newTag);

			// check if query is successful
			if (!queryResult.GetPlyStatus())
			{
				pageFuncData.KLog.Error($"Query insert failed: {queryResult.GetPlyCode}");
				throw new Exception($"Query insert failed: {queryResult.GetPlyCode}");
			}

			var metadata = JsonSerializer.Serialize(queryResult, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			// extract data from query result and post to dynamicElements
			dynamicElements.Add("dym-query-metadata", metadata);

			return dynamicElements;
		}
	}
}

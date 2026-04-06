using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Container.Core;
using Murcex.Vyudro.Module.Client.Models;
using PlyQor.Client;
using System.Text.Json;

namespace Murcex.PlyQor.Internal.Portal.Pages.Query.QueryOperationInternal
{
	public class QuerySelectListPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			var dynamicElements = new Dictionary<string, string>();

			var token = pageFuncData.Auxiliary.GetValue("Token");

			dynamicElements.Add("dym-token", token);

			// get query params from pageFuncData
			// - tag
			// - count
			var tag = pageFuncData.Request.Query["plyqor-tag"];
			if (string.IsNullOrEmpty(tag))
			{
				pageFuncData.KLog.Error("Key not found in query parameters");
				throw new Exception("Key not found in query parameters");
			}

			var count = pageFuncData.Request.Query["plyqor-count"];
			if (string.IsNullOrEmpty(count))
			{
				pageFuncData.KLog.Error("Tag not found in query parameters");
				throw new Exception("Tag not found in query parameters");
			}

			// convert count to int and throw exception if the conversion fails
			if (!int.TryParse(count, out int countValue) || countValue <= 0)
			{
				pageFuncData.KLog.Error("Count must be a positive integer");
				throw new Exception("Count must be a positive integer");
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

			var queryResult = plyQorClient.Select(tag, countValue);

			// check if query is successful
			if (!queryResult.GetPlyStatus())
			{
				pageFuncData.KLog.Error($"Query insert failed: {queryResult.GetPlyCode}");
				throw new Exception($"Query insert failed: {queryResult.GetPlyCode}");
			}

			var data = queryResult.GetPlyData();

			var dataJson = JsonSerializer.Deserialize<List<string>>(data.ToString());

			// Convert the List<string> into a flat comma-separated string "item1,item2,item3"
			var flatData = dataJson == null || dataJson.Count == 0
				? string.Empty
				: string.Join(",", dataJson);

			// Replace Data with the flat string
			queryResult["Data"] = flatData;

			var metadata = JsonSerializer.Serialize(queryResult, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			// extract data from query result and post to dynamicElements
			dynamicElements.Add("dym-query-data", metadata);

			return dynamicElements;
		}
	}
}
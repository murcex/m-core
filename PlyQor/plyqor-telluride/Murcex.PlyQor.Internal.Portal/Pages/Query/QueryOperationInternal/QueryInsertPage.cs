using Murcex.Implements.DataTools.Extensions;
using Murcex.PlyQor.Internal.Container;
using Murcex.PlyQor.Internal.Container.Core;
using Murcex.Vyudro.Module.Client.Models;
using PlyQor.Client;
using System.Text.Json;

namespace Murcex.PlyQor.Internal.Portal.Pages.Query.QueryOperationInternal
{
	public class QueryInsertPage
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			var dynamicElements = new Dictionary<string, string>();

			var token = pageFuncData.Auxiliary.GetValue("Token");

			dynamicElements.Add("dym-token", token);

			// get query params from pageFuncData
			// - key
			// - tag
			// - value -- get from contents
			var key = pageFuncData.Request.Query["plyqor-id"];
			if (string.IsNullOrEmpty(key))
			{
				pageFuncData.KLog.Error("Key not found in query parameters");
				throw new Exception("Key not found in query parameters");
			}

			var tag = pageFuncData.Request.Query["plyqor-tag"];
			if (string.IsNullOrEmpty(tag))
			{
				pageFuncData.KLog.Error("Tag not found in query parameters");
				throw new Exception("Tag not found in query parameters");
			}

			// read value from request body can be null or empty
			// support GET (with data in query) or POST (with body)
			string value;
			if (string.Equals(pageFuncData.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
			{
				value = pageFuncData.Request.Query["plyqor-data"];
			}
			else
			{
				// Use the async reader to avoid synchronous IO on the request stream
				using (var reader = new StreamReader(pageFuncData.Request.Body))
				{
					value = reader.ReadToEndAsync().GetAwaiter().GetResult();
				}
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

			// execute query
			var queryResult = plyQorClient.Insert(key, value, tag);

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
			// expose the query result for templates
			dynamicElements.Add("dym-query-data", metadata);

			return dynamicElements;
		}
	}
}

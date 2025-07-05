using Murcex.Vyudro.Module.Client.Interface;
using PlyQor.Client;

namespace Murcex.Vyudro.Module.Client.StorageAdapters
{
	public class PlyQorAdapter : IStorageAdapter
	{
		private PlyClient _pagePlyClient;

		private PlyClient _sessionPlyClient;

		public PlyQorAdapter(string account, string pageContainer, string pageToken, string sessionContainer, string sessionToken)
		{
			_pagePlyClient = new PlyClient(account, pageContainer, pageToken);

			_sessionPlyClient = new PlyClient(account, sessionContainer, sessionToken);
		}

		public bool GetPage(string page, out string contents, out string message)
		{
			var result = _pagePlyClient.Select(page);

			if (result.GetPlyStatus())
			{
				contents = result.GetPlyData();
				message = "get page complete";
				return true;
			}
			else
			{
				contents = string.Empty;
				var code = result.GetPlyCode();
				message = $"get page failed: {code}";
				return false;
			}
		}

		public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message)
		{
			var result = _sessionPlyClient.Select(manifest);

			userAccess = new();
			if (result.GetPlyStatus())
			{
				var contents = result.GetPlyData();

				var segments = contents.Split("|");
				foreach (var segment in segments)
				{
					var components = segment.Split("=");
					var user = components[0];
					var rawTokens = components[1].Split(",");
					List<string> tokens = new();
					foreach (var token in rawTokens)
					{
						tokens.Add(token);
					}
					userAccess[user] = tokens;
				}

				message = "get user access complete";
				return true;
			}
			else
			{
				userAccess = new Dictionary<string, List<string>>();
				var code = result.GetPlyCode();
				message = $"get user access failed: {code}";
				return false;
			}
		}

		public bool InsertSessionToken(string token, string timestamp, out string message)
		{
			var result = _sessionPlyClient.Insert(token, timestamp, "session");

			if (result.GetPlyStatus())
			{
				message = "insert session token complete";
				return true;
			}
			else
			{
				var code = result.GetPlyCode();
				message = $"insert session token failed: {code}";
				return false;
			}
		}

		public bool GetSessionTokenTimestamp(string token, out string timestamp, out string message)
		{
			var result = _sessionPlyClient.Select(token);

			if (result.GetPlyStatus())
			{
				timestamp = result.GetPlyData();
				message = "get timestamp complete";
				return true;
			}
			else
			{
				timestamp = string.Empty;
				var code = result.GetPlyCode();
				message = $"get timestamp failed: {code}";
				return false;
			}
		}

		public bool DeleteSessionToken(string token, out string message)
		{
			var result = _sessionPlyClient.Delete(token);
			if (result.GetPlyStatus())
			{
				message = "delete session token complete";
				return true;
			}
			else
			{
				var code = result.GetPlyCode();
				message = $"delete session token failed: {code}";
				return false;
			}
		}
	}
}

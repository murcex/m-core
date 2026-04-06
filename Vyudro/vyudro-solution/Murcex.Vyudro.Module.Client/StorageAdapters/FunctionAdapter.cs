using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.Managers;
using System.Collections.Concurrent;

namespace Murcex.Vyudro.Module.Client.StorageAdapters
{
	public class FunctionAdapter : IStorageAdapter
	{
		private ConcurrentDictionary<string, string> _templates = new();

		private ConcurrentDictionary<string, List<string>> _userAccess = new();

		private ConcurrentDictionary<string, string> _sessionTokens = new();

		public FunctionAdapter(Dictionary<string, string> cfg, bool isSession = false)
		{
			if (isSession)
			{
				foreach (var user in cfg)
				{
					if (string.Equals(user.Key, "type", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}

					var tokens = user.Value.Split(",").ToList();
					_userAccess[user.Key] = tokens;
				}
			}
			else
			{
				var templates = ResourceManager.LoadFiles("Templates");
				foreach (var template in templates)
				{
					_templates.TryAdd(template.Key, template.Value);
				}
			}
		}

		public bool GetPage(string page, out string contents, out string message)
		{
			if (_templates.TryGetValue(page, out string? value))
			{
				if (string.IsNullOrEmpty(value))
				{
					contents = string.Empty;
					message = $"Page '{page}' is empty.";
					return false;
				}
				else
				{
					contents = value;
					message = string.Empty;
					return true;
				}
			}
			else
			{
				contents = string.Empty;
				message = $"Page '{page}' not found.";
				return false;
			}
		}

		public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message)
		{
			userAccess = new Dictionary<string, List<string>>(_userAccess);
			message = string.Empty;

			return true;
		}

		public bool InsertSessionToken(string token, string timestamp, out string message)
		{
			if (_sessionTokens.ContainsKey(token))
			{
				message = "token already exists";
				return false;
			}
			else
			{
				if (_sessionTokens.TryAdd(token, timestamp))
				{
					message = "insert succesful";
					return true;
				}
				else
				{
					message = "insert failed";
					return false;
				}
			}
		}

		public bool GetSessionTokenTimestamp(string token, out string timestamp, out string message)
		{
			if (_sessionTokens.TryGetValue(token, out timestamp))
			{
				if (string.IsNullOrEmpty(timestamp))
				{
					message = "get session token timestamp is empty";
					return false;
				}
				else
				{
					message = $"get session token timestamp complete {timestamp}";
					return true;
				}
			}
			else
			{
				message = $"session token not found for {token}";
				timestamp = string.Empty;
				return false;
			}
		}

		public bool DeleteSessionToken(string token, out string message)
		{
			if (_sessionTokens.TryRemove(token, out _))
			{
				message = "token delete complete";
				return true;
			}
			else
			{
				message = "token delete failed";
				return false;
			}
		}
	}
}

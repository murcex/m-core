using Microsoft.AspNetCore.Http;
using Murcex.Vyudro.Internal.Utilities.Extensions;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.StorageAdapters;
using System.Collections.Concurrent;

namespace Murcex.Vyudro.Module.Client.Managers
{
	public class SessionManager
	{
		private ConcurrentDictionary<string, DateTime> _cacheTokens = new ConcurrentDictionary<string, DateTime>();

		private List<string> _invalidToken = new List<string>();

		private int _sessionLimit = 15;

		private IStorageAdapter _storageAdapter;

		private Dictionary<string, List<string>> _userAccess;

		public SessionManager(Dictionary<string, string> storageCfg, IStorageAdapter customerStorageAdapter = null)
		{
			var type = storageCfg.GetValue("type");

			// plyqor
			if (string.Equals("plyqor", type, StringComparison.CurrentCultureIgnoreCase))
			{
				var account = storageCfg.GetValue("account");

				var pageContainer = storageCfg.GetValue("page-container");

				var pageToken = storageCfg.GetValue("page-token");

				var sessionContainer = storageCfg.GetValue("session-container");

				var sessionToken = storageCfg.GetValue("session-token");

				var storageAdapter = new PlyQorAdapter(account, pageContainer, pageToken, sessionContainer, sessionToken);

				_storageAdapter = storageAdapter;
			}

			// web
			if (string.Equals("web", type, StringComparison.CurrentCultureIgnoreCase))
			{
				// setup web request storage adapter
			}

			// local
			if (string.Equals("local", type, StringComparison.OrdinalIgnoreCase))
			{
				var root = storageCfg.GetValue("directory");

				var storageAdapter = new LocalAdapter(root);

				_storageAdapter = storageAdapter;
			}

			// function
			if (string.Equals("function", type, StringComparison.OrdinalIgnoreCase))
			{
				_storageAdapter = new FunctionAdapter(storageCfg, isSession: true);
			}

			// custom
			if (string.Equals("custom", type, StringComparison.OrdinalIgnoreCase) || customerStorageAdapter != null)
			{
				_storageAdapter = customerStorageAdapter;
			}

			var sessiontLimit = storageCfg.GetValue("session-limit");

			if (!string.IsNullOrEmpty(sessiontLimit))
			{
				_sessionLimit = Convert.ToInt32(sessiontLimit);
			}

			var manifest = storageCfg.GetValue("manifest");

			if (string.IsNullOrEmpty(manifest))
			{
				manifest = "manifest.txt";
			}

			if (_storageAdapter.GetUserAccess(manifest, out var userAccess, out string message))
			{
				_userAccess = userAccess;
			}
		}

		public bool CheckAccessToken(HttpRequest request, out string message)
		{
			var accessUser = request.Query["access-user"];
			var accessToken = request.Query["access-token"];

			if (string.IsNullOrEmpty(accessUser))
			{
				message = "access user is empty";
				return false;
			}

			if (string.IsNullOrEmpty(accessToken))
			{
				message = "access token is empty";
				return false;
			}

			if (_userAccess.TryGetValue(accessUser, out var tokens))
			{
				if (tokens.Contains(accessToken))
				{
					message = "access granted";
					return true;
				}
				else
				{
					message = "access token is incorrect";
					return false;
				}
			}
			else
			{
				message = "access user is incorrect";
				return false;
			}
		}

		public bool CheckSessionToken(HttpRequest request, out string token, out string message)
		{
			token = request.Query["session-token"];

			if (string.IsNullOrEmpty(token))
			{
				message = "token empty";
				return false;
			}

			var timestamp = DateTime.MinValue;

			if (_cacheTokens.TryGetValue(token, out timestamp))
			{
				if (IsSessionTokenExpired(timestamp))
				{
					_cacheTokens.Remove(token, out _);
					_invalidToken.Add(token);
					message = "token expired";
					return false;
				}
				else
				{
					message = "token approved";
					return true;
				}
			}
			else
			{
				if (_invalidToken.Contains(token))
				{
					message = "token invalid";
					return false;
				}
				else
				{
					if (GetSessionTokenTimeStamp(token, out timestamp))
					{
						if (IsSessionTokenExpired(timestamp))
						{
							_invalidToken.Add(token);
							message = "token expired";
							return false;
						}
						else
						{
							_cacheTokens.TryAdd(token, timestamp);
							message = "token approved";
							return true;
						}
					}
					else
					{
						_invalidToken.Add(token);
						message = "token invalid";
						return false;
					}
				}
			}
		}

		public bool CreateSessionToken(HttpRequest request, out string token, out string message)
		{
			token = Guid.NewGuid().ToString();
			var timestamp = DateTime.UtcNow.AddMinutes(_sessionLimit);

			if (_storageAdapter.InsertSessionToken(token, timestamp.ToString(), out string adapterMessage))
			{
				_cacheTokens.TryAdd(token, timestamp);

				message = $"token created: {adapterMessage}";
				return true;
			}
			else
			{
				message = $"token failed to created: {adapterMessage}";
				return false;
			}
		}

		public bool DisableSessionToken(HttpRequest request, out string token, out string message)
		{
			token = request.Query["session-token"];

			if (string.IsNullOrEmpty(token))
			{
				message = "session token is empty";
				return false;
			}

			if (DeleteSessionToken(token, out var opMessage))
			{
				_cacheTokens.Remove(token, out _);

				_invalidToken.Add(token);

				message = opMessage;
				return true;
			}
			else
			{
				message = opMessage;
				return false;
			}
		}

		private bool IsSessionTokenExpired(DateTime timestamp)
		{
			if (timestamp < DateTime.UtcNow.AddMinutes(-15))
			{
				return true;
			}

			return false;
		}

		private bool GetSessionTokenTimeStamp(string token, out DateTime expiration)
		{
			if (_storageAdapter.GetSessionTokenTimestamp(token, out string timestampValue, out string message))
			{

				if (string.IsNullOrEmpty(timestampValue))
				{
					expiration = DateTime.MinValue;
					return false;
				}

				expiration = Convert.ToDateTime(timestampValue);

				return true;
			}
			else
			{
				expiration = DateTime.MinValue;
				return false;
			}
		}

		private bool DeleteSessionToken(string token, out string message)
		{
			if (_storageAdapter.DeleteSessionToken(token, out string adapterMessage))
			{
				message = $"session token has been deleted: {adapterMessage}";
				return true;
			}
			else
			{
				message = $"failed to delete session token: {adapterMessage}";
				return false;
			}
		}
	}
}

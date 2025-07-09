using Murcex.Vyudro.Module.Client.Interface;

namespace Murcex.Vyudro.Test.Client.Components
{
	internal class TestStorageAdapter : IStorageAdapter
	{
		private Dictionary<string, string> _sessionTokens;

		public TestStorageAdapter()
		{
			_sessionTokens = new();
		}

		public bool DeleteSessionToken(string token, out string message)
		{
			if (_sessionTokens.Remove(token))
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

		public bool GetPage(string page, out string content, out string message)
		{
			if (string.Equals("test.html", page, StringComparison.OrdinalIgnoreCase))
			{
				message = "test";
				content = "<!DOCTYPE html>\r\n<html>\r\n\r\n<head>\r\n    <title>Azure Function HTML Response</title>\r\n</head>\r\n\r\n<body>\r\n    <h1>Hello from Azure Function! (customer adapater)</h1>\r\n    <p>This is an HTML response from an Azure HTTP trigger function.</p>\r\n    <p>$$$global$$$</p>\r\n    <p>$$$static$$$</p>\r\n    <p>$$$dynamic$$$</p>\r\n</body>\r\n\r\n</html>";
				return true;
			}
			else if (string.Equals("test2.html", page, StringComparison.OrdinalIgnoreCase))
			{
				message = "test";
				content = "<!DOCTYPE html>\r\n<html>\r\n\r\n<head>\r\n    <title>Azure Function HTML Response</title>\r\n</head>\r\n\r\n<body>\r\n    <h1>Hello from Azure Function! (customer adapater)</h1>\r\n    <p>This is an HTML response from an Azure HTTP trigger function.</p>\r\n    <p>$$$global$$$</p>\r\n    <p>$$$static$$$</p>\r\n    <p>$$$dynamic$$$</p>\r\n</body>\r\n\r\n</html>";
				return true;
			}
			else
			{
				message = "";
				content = string.Empty;
				return false;
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
					message = "get session token timestamp complete";
					return true;
				}
			}
			else
			{
				message = "session token not found";
				return false;
			}
		}

		public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message)
		{
			userAccess = new();

			List<string> tokens = ["unittestpassword"];

			userAccess.Add("unittestuser", tokens);

			message = "manifest generated";
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
	}
}

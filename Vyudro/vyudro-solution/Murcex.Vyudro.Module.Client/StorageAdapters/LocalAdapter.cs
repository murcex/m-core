using Murcex.Implements.DataTools.IO;
using Murcex.Vyudro.Module.Client.Interface;

namespace Murcex.Vyudro.Module.Client.StorageAdapters
{
	public class LocalAdapter : IStorageAdapter
	{
		private string _root;

		public LocalAdapter(string root)
		{
			_root = root;

			var options = new FileManagerOptions();
			options.ThrowException = true;
			options.SetRootDirectory(root);

			FileManager.SetOptions(options);
		}

		public bool GetPage(string page, out string content, out string message)
		{
			var file = Path.Combine(_root, page);

			var lines = FileManager.ReadFile(file, root: false);

			content = string.Join("", lines);

			if (string.IsNullOrEmpty(content))
			{
				message = "page contents empty";
			}
			else
			{
				message = "get page complete";
			}

			return true;
		}

		public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message)
		{
			var file = Path.Combine(_root, $"{manifest}");

			var lines = FileManager.ReadFile(file);

			userAccess = new();
			foreach (var line in lines)
			{
				var components = line.Split('=');
				var user = components[0];
				var tokens = components[1].Split(",");

				List<string> tokens2 = new();
				foreach (var token in tokens)
				{
					tokens2.Add(token);
				}

				userAccess[user] = tokens2;
			}

			message = "user access generated";
			return true;
		}

		public bool InsertSessionToken(string token, string timestamp, out string message)
		{
			var file = Path.Combine(_root, $"{token}.txt");

			if (!FileManager.FileExists(file))
			{
				List<string> lines = new List<string>
				{
					timestamp
				};

				if (FileManager.CreateFile(file, lines))
				{
					message = "insert complete";
					return true;
				}
				else
				{
					message = "insert failed";
					return false;
				}
			}
			else
			{
				message = "file already exist";
				return false;
			}
		}

		public bool GetSessionTokenTimestamp(string token, out string timestamp, out string message)
		{
			var file = Path.Combine(_root, $"{token}.txt");

			var lines = FileManager.ReadFile(file);

			timestamp = string.Join("", lines);

			if (string.IsNullOrEmpty(timestamp))
			{
				message = "get session token timestamp is empty";
			}
			else
			{
				message = "get session token timestamp complete";
			}

			return true;
		}

		public bool DeleteSessionToken(string token, out string message)
		{
			var file = Path.Combine(_root, $"{token}.txt");

			if (FileManager.DeleteFile(file))
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

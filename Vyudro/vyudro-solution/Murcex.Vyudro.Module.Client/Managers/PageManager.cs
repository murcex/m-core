using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.StorageAdapters;

namespace Murcex.Vyudro.Module.Client.Managers
{
	public class PageManager
	{
		private IStorageAdapter _storageAdapter;

		public PageManager(Dictionary<string, string> storageCfg, IStorageAdapter storageAdapter = null)
		{

			if (storageAdapter != null)
			{
				_storageAdapter = storageAdapter;

				return;
			}

			var type = storageCfg.GetValue("type");

			// plyqor
			if (string.Equals("plyqor", type, StringComparison.OrdinalIgnoreCase))
			{
				var account = storageCfg.GetValue("account");

				var pageContainer = storageCfg.GetValue("page-container");

				var pageToken = storageCfg.GetValue("page-token");

				var sessionContainer = storageCfg.GetValue("session-container");

				var sessionToken = storageCfg.GetValue("session-token");

				_storageAdapter = new PlyQorAdapter(account, pageContainer, pageToken, sessionContainer, sessionToken);

				return;
			}

			// web
			if (string.Equals("web", type, StringComparison.OrdinalIgnoreCase))
			{
				var pageUrl = storageCfg.GetValue("page-url");

				var sessionUrl = storageCfg.GetValue("session-url");

				var replace = storageCfg.GetValue("input-place-holder");

				//var storageAdapter = new WebAdapter(pageUrl, sessionUrl, replace);

				//_storageAdapter = storageAdapter;
			}

			// local
			if (string.Equals("local", type, StringComparison.OrdinalIgnoreCase))
			{
				var root = storageCfg.GetValue("directory");

				_storageAdapter = new LocalAdapter(root);

				return;
			}

			// function
			if (string.Equals("function", type, StringComparison.OrdinalIgnoreCase))
			{
				_storageAdapter = new FunctionAdapter(storageCfg, isSession: false);
			}
		}

		public bool GetPage(string name, out string content, out string message)
		{
			return _storageAdapter.GetPage(name, out content, out message);
		}
	}
}

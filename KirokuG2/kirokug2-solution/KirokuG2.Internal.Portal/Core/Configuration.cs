using Murcex.Vyudro.Module.Client;

namespace KirokuG2.Internal.Portal.Core
{
	public class Configuration
	{
		public static string KirokuUrl => $"https://{_kqueryUrl}/api/Query?token={_kqueryToken}&id=";

		public static VyudroManager SiteManager => _siteManager;

		private static string _kqueryUrl;

		private static string _kqueryToken;

		private static VyudroManager _siteManager;

		public static void Load(Dictionary<string, Dictionary<string, string>> cfg)
		{
			// get kqueryCfg dictionary<string,string from cfg as key "kquery"
			if (cfg.TryGetValue("kquery", out var kqueryCfg))
			{
				// get kquery-url key from cfg assign to _kqueryUrl
				if (kqueryCfg.TryGetValue("kquery-url", out var url))
				{
					_kqueryUrl = url;
				}

				// get kquery-token key from cfg assign to _kqueryToken
				if (kqueryCfg.TryGetValue("kquery-token", out var token))
				{
					_kqueryToken = token;
				}
			}
		}

		public static void AddSiteManager(VyudroManager siteManager)
		{
			_siteManager = siteManager;
		}
	}
}

using Murcex.Vyudro.Module.Client;

namespace KirokuG2.Internal.Portal.Core
{
	public class Configuration
	{
		public static VyudroManager SiteManager => _siteManager;

		private static VyudroManager _siteManager;

		public static void AddSiteManager(VyudroManager siteManager)
		{
			_siteManager = siteManager;
		}
	}
}

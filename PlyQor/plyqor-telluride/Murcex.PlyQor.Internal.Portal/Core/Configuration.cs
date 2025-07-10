using Murcex.Vyudro.Module.Client;

namespace Murcex.PlyQor.Internal.Portal.Core
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

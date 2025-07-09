using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Managers;
using Murcex.Vyudro.Module.Client.Models;
using System.Collections.Concurrent;

namespace Murcex.Vyudro.Module.Client.Core
{
	public class Configuration
	{
		public Dictionary<string, string> GlobalElements { get; private set; }

		public ConcurrentDictionary<string, Page> PageCache { get; private set; }

		public Dictionary<ErrorType, string> ErrorPages { get; private set; }

		public PageManager PageManager { get; private set; }

		public SessionManager SessionManager { get; private set; }

		public Configuration(
			Dictionary<string, string> _globalElements,
			ConcurrentDictionary<string, Page> _pageCache,
			Dictionary<ErrorType, string> _errorPages,
			PageManager _pageManager,
			SessionManager _sessionManager)
		{
			GlobalElements = _globalElements;
			PageCache = _pageCache;
			ErrorPages = _errorPages;
			PageManager = _pageManager;
			SessionManager = _sessionManager;
		}
	}
}

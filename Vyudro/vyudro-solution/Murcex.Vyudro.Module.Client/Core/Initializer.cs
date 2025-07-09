using Murcex.Implements.DataTools.Extensions;
using Murcex.Vyudro.Module.Client.Enum;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Module.Client.Managers;
using Murcex.Vyudro.Module.Client.Models;
using System.Collections.Concurrent;

namespace Murcex.Vyudro.Module.Client.Core
{
	public class Initializer
	{
		public static Configuration Execute(Dictionary<string, Dictionary<string, string>> cfg,
			Func<
			// input
			PageFuncData,
			// return
			Dictionary<string, string>> dynamicPageFunc,
			IStorageAdapter storageAdapter = null)
		{
			// get [site]
			var siteCfg = cfg.GetValue("site", keyEx: "site config is missing", valueEx: "site config is null");

			// 1) setup page manager
			var storageCfg = cfg.GetValue("page-manager");
			PageManager pageManager = new PageManager(storageCfg, storageAdapter);

			// 2) setup session manager
			var sessionStorageCfg = cfg.GetValue("session-manager");
			SessionManager sessionManager = new SessionManager(sessionStorageCfg, storageAdapter);

			// 3) get staticElements
			var staticElements = new Dictionary<string, string>();
			if (cfg.TryGetValue("elements", out var globalElemensValue))
			{
				foreach (var globalElementKey in globalElemensValue)
				{
					var key = globalElementKey.Key;
					var value = globalElementKey.Value;

					staticElements.Add(key, value);
				}
			}

			// 4) get azure function hostname
			var hostName = GetHostName();

			staticElements.Add("WEBSITE_HOSTNAME", hostName ?? string.Empty);

			// 5) get images from function and apply to staticElements
			var imageType = siteCfg.GetValue("images");
			if (!string.IsNullOrEmpty(imageType))
			{
				if (string.Equals(imageType, "function", StringComparison.OrdinalIgnoreCase))
				{
					var loadBase64Images = ResourceManager.LoadFiles("Images", image: true);
					foreach (var staticElement in staticElements)
					{
						if (loadBase64Images.TryGetValue(staticElement.Key, out var base64Image))
						{
							staticElements[staticElement.Key] = base64Image;
						}
					}
				}
			}

			// 6) setup error pages
			var errorPages = CreateErrorPageDataSet(cfg);

			// 7) get [pages]
			var pageKeys = siteCfg.GetValue("pages").ToList();

			// 8) foreach page in pages
			var pageCache = new ConcurrentDictionary<string, Page>();
			var firstPage = true;
			foreach (var pageKey in pageKeys)
			{
				// get page metadata
				var pageValue = cfg.GetValue(pageKey);
				var name = pageValue.GetValue("name");
				if (string.IsNullOrEmpty(name))
				{
					name = pageKey;
				}
				var file = pageValue.GetValue("file");

				var page = new Page(name, file);

				// set page security
				var authModeValue = pageValue.GetValue("security");

				PageSecurityType authType = GetPageSecurity(authModeValue);

				page.AddPageSecurity(authType.ToString());

				// get page func and add
				if (firstPage)
				{
					page.AddPageFunction(dynamicPageFunc);

					firstPage = false;
				}

				// get page contents from storage
				pageManager.GetPage(page.File, out string content, out string message);

				page.AddPageContent(content);

				// if elements is provided, apply only those static elements to page
				if (pageValue.TryGetValue("elements", out var staticElementsValue))
				{
					var pageStaticElements = staticElementsValue.ToList();

					foreach (var pageStaticElement in pageStaticElements)
					{
						if (staticElements.TryGetValue(pageStaticElement, out var element))
						{
							page.ApplyStaticElement(pageStaticElement, element);
						}
					}
				}
				else // default operation - if elements param not provided, apply all static elements to page
				{
					foreach (var staticElement in staticElements)
					{
						page.ApplyStaticElement(staticElement.Key, staticElement.Value);
					}
				}

				page.UpdatePageState(PageStateType.Online);

				pageCache.TryAdd(page.Name, page);
			}

			return new Configuration(staticElements, pageCache, errorPages, pageManager, sessionManager);
		}

		private static string GetHostName()
		{
			string hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

			string env = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

			var prefix = "https://";
			if (string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
			{
				prefix = "http://";
			}

			hostName = $"{prefix}{hostName}";

			return hostName;
		}

		private static Dictionary<ErrorType, string> CreateErrorPageDataSet(Dictionary<string, Dictionary<string, string>> cfg)
		{
			var errorPages = new Dictionary<ErrorType, string>();
			if (cfg.TryGetValue("error-pages", out var errorPagesCfg))
			{
				foreach (var errorPageCfg in errorPagesCfg)
				{
					ErrorType errorType = ErrorType.None;
					if (string.Equals(errorPageCfg.Key, "404", StringComparison.OrdinalIgnoreCase))
					{
						errorType = ErrorType.NotFound;
					}
					else if (string.Equals(errorPageCfg.Key, "403", StringComparison.OrdinalIgnoreCase))
					{
						errorType = ErrorType.Access;
					}
					else if (string.Equals(errorPageCfg.Key, "400", StringComparison.OrdinalIgnoreCase))
					{
						errorType = ErrorType.Invalid;
					}
					else if (string.Equals(errorPageCfg.Key, "500", StringComparison.OrdinalIgnoreCase))
					{
						errorType = ErrorType.Exception;
					}

					if (errorType != ErrorType.None)
					{
						errorPages.Add(errorType, errorPageCfg.Value);
					}
				}
			}

			return errorPages;
		}

		private static PageSecurityType GetPageSecurity(string authModeValue)
		{
			PageSecurityType authType = PageSecurityType.Token;

			if (string.IsNullOrEmpty(authModeValue))
			{
				authType = PageSecurityType.Token;
			}
			else if (string.Equals(authModeValue, "none", StringComparison.OrdinalIgnoreCase))
			{
				authType = PageSecurityType.None;
			}
			else if (string.Equals(authModeValue, "login", StringComparison.OrdinalIgnoreCase))
			{
				authType = PageSecurityType.Login;
			}
			else if (string.Equals(authModeValue, "logout", StringComparison.OrdinalIgnoreCase))
			{
				authType = PageSecurityType.Logout;
			}
			else if (string.Equals(authModeValue, "token", StringComparison.OrdinalIgnoreCase))
			{
				authType = PageSecurityType.Token;
			}

			return authType;
		}
	}
}

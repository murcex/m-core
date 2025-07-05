using Murcex.Vyudro.Module.Client;
using Murcex.Vyudro.Module.Client.Interface;
using Murcex.Vyudro.Test.Client.Components;
using System.Text.RegularExpressions;

namespace Murcex.Vyudro.Test.Client
{
	[TestClass]
	public sealed class Index
	{
		private static string _pageDirectory = "";

		private static string _sessionDirectory = "";

		[AssemblyInitialize]
		public static void AssemblyInit(TestContext context)
		{
			// Dynamically get the path to the Core\Config.ini file relative to the test assembly location
			string assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
			string configPath = Path.Combine(assemblyDir, "Core", "Config.ini");

			// Example: Read all lines from Config.ini if it exists
			string[] configLines = Array.Empty<string>();
			if (File.Exists(configPath))
			{
				configLines = File.ReadAllLines(configPath);
				// You can parse or use configLines as needed
			}

			_pageDirectory = Path.Combine(configLines[0], "pages");

			_sessionDirectory = Path.Combine(configLines[0], "session-tokens");

			if (!Directory.Exists(_pageDirectory))
			{
				Directory.CreateDirectory(_pageDirectory);

				// Copy all files from Resources\Pages to _pageDirectory
				string sourceDir = Path.Combine(assemblyDir, "Resources", "Pages");

				if (Directory.Exists(sourceDir))
				{
					foreach (var file in Directory.GetFiles(sourceDir))
					{
						string destFile = Path.Combine(_pageDirectory, Path.GetFileName(file));
						File.Copy(file, destFile, overwrite: true);
					}
				}
			}

			if (!Directory.Exists(_sessionDirectory))
			{
				Directory.CreateDirectory(_sessionDirectory);

				// Copy all files from Resources\Session to _sessionDirectory
				string sessionSourceDir = Path.Combine(assemblyDir, "Resources", "Session");

				if (Directory.Exists(sessionSourceDir))
				{
					foreach (var file in Directory.GetFiles(sessionSourceDir))
					{
						string destFile = Path.Combine(_sessionDirectory, Path.GetFileName(file));
						File.Copy(file, destFile, overwrite: true);
					}
				}
			}

			// Delete all files in _sessionDirectory except manifest.txt
			if (Directory.Exists(_sessionDirectory))
			{
				foreach (var file in Directory.GetFiles(_sessionDirectory))
				{
					if (!string.Equals(Path.GetFileName(file), "manifest.txt", StringComparison.OrdinalIgnoreCase))
					{
						File.Delete(file);
					}
				}
			}
		}

		[TestMethod]
		public void PageNoAuthNoFuncNoElements()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();

			siteCfg["pages"] = "page1";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			cfg.Add("session-manager", sessionStorageConfig);

			page1["name"] = "test";
			page1["file"] = "test.html";
			page1["security"] = "none";
			cfg.Add("page1", page1);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, null);

			var request = Utilities.GetHttpRequest();

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function!</h1>"));
		}

		[TestMethod]
		public void PageNoAuthWithGlobalAndStaticElements()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();

			Dictionary<string, string> globalElements = new();

			siteCfg["pages"] = "page1";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			cfg.Add("session-manager", sessionStorageConfig);

			page1["name"] = "test";
			page1["file"] = "test.html";
			page1["security"] = "none";
			page1["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page1", page1);

			globalElements.Add("$$$global$$$", "GLOBALREPLACE");
			globalElements.Add("$$$static$$$", "STATICREPLACE");
			cfg.Add("elements", globalElements);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, null);

			var request = Utilities.GetHttpRequest();

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function!</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
		}

		[TestMethod]
		public void PageNoAuthWithGlobalAndStaticAndDynamicElements()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();

			Dictionary<string, string> globalElements = new();

			siteCfg["pages"] = "page1";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			cfg.Add("session-manager", sessionStorageConfig);

			page1["name"] = "test";
			page1["file"] = "test.html";
			page1["security"] = "none";
			page1["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page1", page1);

			globalElements.Add("$$$global$$$", "GLOBALREPLACE");
			globalElements.Add("$$$static$$$", "STATICREPLACE");
			cfg.Add("elements", globalElements);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, PageFuncs.PageFuncSelector);

			var request = Utilities.GetHttpRequest();

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function!</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>123INPUTGLOBALREPLACESTATICREPLACE</p>"));
		}

		[TestMethod]
		public void PageSessionManagerTest()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();
			Dictionary<string, string> page2 = new();
			Dictionary<string, string> page3 = new();

			Dictionary<string, string> globalElements = new();

			siteCfg["pages"] = "page1,page2,page3";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			sessionStorageConfig["session-limit"] = "15";
			cfg.Add("session-manager", sessionStorageConfig);

			globalElements.Add("$$$global$$$", "GLOBALREPLACE");
			globalElements.Add("$$$static$$$", "STATICREPLACE");
			cfg.Add("elements", globalElements);

			page1["name"] = "test-login";
			page1["file"] = "test.html";
			page1["security"] = "login";
			page1["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page1", page1);

			page2["name"] = "test";
			page2["file"] = "test.html";
			page2["security"] = "token";
			page2["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page2", page2);

			page3["name"] = "test-logout";
			page3["file"] = "test.html";
			page3["security"] = "logout";
			page3["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page3", page3);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, PageFuncs.PageFuncSelector);

			// login
			var request = Utilities.GetHttpRequest(page: "test-login");

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function!</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>SESSIONTOKEN_"));

			// token check
			// Define the regex to find GUIDs following TEST_
			string pattern = @"SESSIONTOKEN_(?<guid>[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})";
			// Create a regex object
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			// Match the regex pattern in the file content MatchCollection matches
			var match = regex.Match(page.Content);

			var sessionToken = string.Empty;
			if (match.Success)
			{
				sessionToken = match.Value;
			}

			sessionToken = sessionToken.Replace("SESSIONTOKEN_", "");

			request = Utilities.GetHttpRequest(page: "test", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function!</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>123INPUTGLOBALREPLACESTATICREPLACE</p>"));

			// logout
			request = Utilities.GetHttpRequest(page: "test-logout", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			// recheck revoked token
			request = Utilities.GetHttpRequest(page: "test", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("AccessError"));
		}

		[TestMethod]
		public void PathNotFound()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();

			siteCfg["pages"] = "page1";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			cfg.Add("session-manager", sessionStorageConfig);

			page1["name"] = "test1";
			page1["file"] = "test.html";
			page1["auth-type"] = "none";
			cfg.Add("page1", page1);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, null);

			var request = Utilities.GetHttpRequest(page: "blah");

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("PageNotFound"));
		}

		[TestMethod]
		public void CustomErrorPages()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> errorPages = new();
			Dictionary<string, string> page1 = new();
			Dictionary<string, string> page2 = new();
			Dictionary<string, string> page3 = new();
			Dictionary<string, string> page4 = new();
			Dictionary<string, string> page5 = new();

			siteCfg["pages"] = "page1,page2,page3,page4,page5";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "local";
			pageStorageCfg["directory"] = _pageDirectory;
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "local";
			sessionStorageConfig["directory"] = _sessionDirectory;
			cfg.Add("session-manager", sessionStorageConfig);

			errorPages.Add("404", "error-not-found");
			errorPages.Add("500", "error-exception");
			errorPages.Add("403", "error-access");
			cfg.Add("error-pages", errorPages);

			page1["name"] = "error-not-found";
			page1["file"] = "error-not-found.html";
			page1["security"] = "none";
			cfg.Add("page1", page1);

			page2["name"] = "kaboom";
			page2["file"] = "kaboom.html";
			page2["security"] = "none";
			cfg.Add("page2", page2);

			page3["name"] = "error-exception";
			page3["file"] = "error-exception.html";
			page3["security"] = "none";
			cfg.Add("page3", page3);

			page4["name"] = "error-access";
			page4["file"] = "error-access.html";
			page4["security"] = "none";
			cfg.Add("page4", page4);

			page5["name"] = "test";
			page5["file"] = "test.html";
			page5["security"] = "login";
			cfg.Add("page5", page5);

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, PageFuncs.PageFuncSelector);

			var request = Utilities.GetHttpRequest(page: "blah");

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<title>Custom Not Found Page</title>"));
			Assert.IsTrue(page.Content.Contains("<h1>Custom Message</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>Page blah Not Found</p>"));

			// exception
			request = Utilities.GetHttpRequest(page: "kaboom");

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<title>Custom Exception Page</title>"));
			Assert.IsTrue(page.Content.Contains("<h1>Custom Message</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>KABOOM!</p>"));

			// access
			request = Utilities.GetHttpRequest(page: "test", login: "wrongtoken");

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<title>Custom Access Page</title>"));
			Assert.IsTrue(page.Content.Contains("<h1>Custom Message</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>access token is incorrect</p>"));
		}

		[TestMethod]
		public void PageSessionManagerCustomAdapterTest()
		{
			Dictionary<string, Dictionary<string, string>> cfg = new();

			Dictionary<string, string> siteCfg = new();
			Dictionary<string, string> pageStorageCfg = new();
			Dictionary<string, string> sessionStorageConfig = new();
			Dictionary<string, string> page1 = new();
			Dictionary<string, string> page2 = new();
			Dictionary<string, string> page3 = new();

			Dictionary<string, string> globalElements = new();

			siteCfg["pages"] = "page1,page2,page3";
			cfg.Add("site", siteCfg);

			pageStorageCfg["type"] = "custom";
			cfg.Add("page-manager", pageStorageCfg);

			sessionStorageConfig["type"] = "custom";
			sessionStorageConfig["session-limit"] = "15";
			cfg.Add("session-manager", sessionStorageConfig);

			globalElements.Add("$$$global$$$", "GLOBALREPLACE");
			globalElements.Add("$$$static$$$", "STATICREPLACE");
			cfg.Add("elements", globalElements);

			page1["name"] = "test-login";
			page1["file"] = "test.html";
			page1["security"] = "login";
			page1["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page1", page1);

			page2["name"] = "test";
			page2["file"] = "test.html";
			page2["security"] = "token";
			page2["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page2", page2);

			page3["name"] = "test-logout";
			page3["file"] = "test.html";
			page3["security"] = "logout";
			page3["elements"] = "$$$global$$$,$$$static$$$";
			cfg.Add("page3", page3);

			IStorageAdapter testAdapter = new TestStorageAdapter();

			var siteManager = new VyudroManager();
			siteManager.Initialize(cfg, PageFuncs.PageFuncSelector, testAdapter);

			// login
			var request = Utilities.GetHttpRequest(page: "test-login");

			var page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function! (customer adapater)</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>SESSIONTOKEN_"));

			// token check
			// Define the regex to find GUIDs following TEST_
			string pattern = @"SESSIONTOKEN_(?<guid>[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})";
			// Create a regex object
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			// Match the regex pattern in the file content MatchCollection matches
			var match = regex.Match(page.Content);

			var sessionToken = string.Empty;
			if (match.Success)
			{
				sessionToken = match.Value;
			}

			sessionToken = sessionToken.Replace("SESSIONTOKEN_", "");

			request = Utilities.GetHttpRequest(page: "test", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("<h1>Hello from Azure Function! (customer adapater)</h1>"));
			Assert.IsTrue(page.Content.Contains("<p>GLOBALREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>STATICREPLACE</p>"));
			Assert.IsTrue(page.Content.Contains("<p>123INPUTGLOBALREPLACESTATICREPLACE</p>"));

			// logout
			request = Utilities.GetHttpRequest(page: "test-logout", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			// recheck revoked token
			request = Utilities.GetHttpRequest(page: "test", sessionToken: sessionToken);

			page = siteManager.GeneratePage(request);

			Assert.IsTrue(page.Content.Contains("AccessError"));
		}
	}
}

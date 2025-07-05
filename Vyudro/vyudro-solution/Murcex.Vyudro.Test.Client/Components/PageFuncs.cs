using Murcex.Vyudro.Internal.Utilities.Extensions;
using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Test.Client.Components
{
	public class PageFuncs
	{
		public static Dictionary<string, string> PageFuncSelector(PageFuncData pageFuncData)
		{
			var page = pageFuncData.Auxiliary.GetValue("Page");

			if (string.IsNullOrEmpty(page))
			{
				pageFuncData.KLog.Error("Page name not found");
				throw new Exception("Page name not found");
			}

			switch (page)
			{
				case "test":
					return Test_DefaultPage(pageFuncData);
				case "test-login":
					return Test_LoginPage(pageFuncData);
				case "test-logout":
					return new Dictionary<string, string>();
				case "error-not-found":
					return Test_CustomErrorPage(pageFuncData);
				case "kaboom":
					return Test_ThrowException(pageFuncData);
				case "error-exception":
					return Test_CustomErrorPage(pageFuncData);
				case "error-access":
					return Test_CustomErrorPage(pageFuncData);
				default:
					pageFuncData.KLog.Error($"Page Function not found for {page}");
					throw new Exception($"Page Function not found for {page}");
			}
		}

		public static Dictionary<string, string> Test_DefaultPage(PageFuncData pageFuncData)
		{
			// get from request
			var part1 = pageFuncData.Request.Query["input"];

			// get global
			pageFuncData.Elements.TryGetValue("$$$global$$$", out var part2);

			// get static
			pageFuncData.Elements.TryGetValue("$$$static$$$", out var part3);

			// combine all and return inside dict
			var results = new Dictionary<string, string>
			{
				{ "$$$dynamic$$$", $"123{part1}{part2}{part3}" }
			};

			return results;
		}

		public static Dictionary<string, string> Test_LoginPage(PageFuncData pageFuncData)
		{
			// get from request
			var part1 = pageFuncData.Auxiliary.GetValue("Token");

			// combine all and return inside dict
			var results = new Dictionary<string, string>
			{
				{ "$$$dynamic$$$", $"SESSIONTOKEN_{part1}" }
			};

			return results;
		}

		public static Dictionary<string, string> Test_CustomErrorPage(PageFuncData pageFuncData)
		{
			var results = new Dictionary<string, string>
			{
				{ "dym-placeholder", $"{pageFuncData.Auxiliary.GetValue("Message")}" }
			};

			return results;
		}

		public static Dictionary<string, string> Test_ThrowException(PageFuncData pageFuncData)
		{
			throw new Exception($"KABOOM!");
		}

	}
}

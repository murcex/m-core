using Murcex.Vyudro.Module.Client.Models;

namespace Murcex.Vyudro.Internal.Audit.Pages
{
	public class TestSigninPage
	{
		public static Dictionary<string, string> ExecuteSignin(PageFuncData pageFuncData)
		{
			pageFuncData.KLog.Trace($"Executing Signin Page Func");

			var dynamicElements = new Dictionary<string, string>
			{
				//{ "dym-background", ElementManager.GetDynamicElement("dym-background") }
			};

			return dynamicElements;
		}
	}
}

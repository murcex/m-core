using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirokuG2.Internal.Portal
{
	public class PageFuncSelector
	{
		public static Dictionary<string, string> Execute(PageFuncData pageFuncData)
		{
			var page = pageFuncData.Auxiliary.GetValue("Page");

			if (string.IsNullOrEmpty(page))
			{
				pageFuncData.KLog.Error("Page name not found");
				throw new Exception("Page name not found");
			}

			switch (page)
			{
				case "test-signin":
					return new Dictionary<string, string>();
				default:
					pageFuncData.KLog.Error($"Page Function not found for {page}");
					throw new Exception($"Page Function not found for {page}");
			}
		}
	}
}

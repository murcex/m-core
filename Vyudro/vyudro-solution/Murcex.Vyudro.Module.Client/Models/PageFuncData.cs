using KirokuG2;
using Microsoft.AspNetCore.Http;

namespace Murcex.Vyudro.Module.Client.Models
{
	public class PageFuncData
	{
		public HttpRequest Request { get; private set; }

		public Dictionary<string, string> Elements { get; private set; }

		public IKLog KLog { get; private set; }

		public Dictionary<string, string> Auxiliary { get; private set; }

		public PageFuncData(
			HttpRequest request,
			Dictionary<string, string> elements,
			IKLog klog,
			Dictionary<string, string> auxiliary)
		{
			Request = request;
			Elements = elements;
			KLog = klog;
			Auxiliary = auxiliary;
		}
	}
}

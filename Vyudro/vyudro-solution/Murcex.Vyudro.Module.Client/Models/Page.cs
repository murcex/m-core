using KirokuG2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murcex.Vyudro.Module.Client.Enum;

namespace Murcex.Vyudro.Module.Client.Models
{
	public class Page
	{
		public string Name { get; private set; }

		public string File { get; private set; }

		public PageStateType PageState { get; private set; }

		public PageSecurityType AuthType => _authType;

		string _content = string.Empty;

		private PageSecurityType _authType;

		private static Func<
			// input
			PageFuncData,
			// return
			Dictionary<string, string>> _dynamicFunc;

		public Page(string name, string file)
		{
			Name = name;
			File = file;
			PageState = PageStateType.Offline;
		}

		public void AddPageSecurity(string security)
		{
			if (string.Equals(security, PageSecurityType.Token.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				_authType = PageSecurityType.Token;
			}
			else if (string.Equals(security, PageSecurityType.None.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				_authType = PageSecurityType.None;
			}
			else if (string.Equals(security, PageSecurityType.Login.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				_authType = PageSecurityType.Login;
			}
			else if (string.Equals(security, PageSecurityType.Logout.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				_authType = PageSecurityType.Logout;
			}
			else
			{
				_authType = PageSecurityType.Token;
			}
		}

		public void AddPageFunction(Func<
			// input
			PageFuncData,
			// return
			Dictionary<string, string>> dynamicPageFunc)
		{
			_dynamicFunc = dynamicPageFunc;
		}

		public void AddPageContent(string content)
		{
			_content = content;
		}

		public void UpdatePageState(PageStateType pageStateType)
		{
			PageState = pageStateType;
		}

		public void ApplyStaticElement(string elementKey, string elementValue)
		{
			Replace(elementKey, elementValue);
		}

		public ContentResult GeneratePage(HttpRequest request, Dictionary<string, string> elements, IKLog klog, Dictionary<string, string> auxiliary)
		{
			var dynamicFuncElements = new Dictionary<string, string>();

			if (_dynamicFunc != null)
			{
				dynamicFuncElements = _dynamicFunc(new PageFuncData(request, elements, klog, auxiliary));
			}

			var content = new string(_content);

			foreach (var pageElement in dynamicFuncElements)
			{
				content = content.Replace(pageElement.Key, pageElement.Value);
			}

			klog.Trace($"Page Generated");

			return new ContentResult()
			{
				Content = content,
				ContentType = "text/html",
				StatusCode = 200
			};
		}

		private void Replace(string target, string update)
		{
			_content = _content.Replace(target, update);
		}
	}
}

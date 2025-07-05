using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Murcex.Vyudro.Test.Client.Components
{
	public class Utilities
	{
		public static HttpRequest GetHttpRequest(string page = null, string login = null, string sessionToken = null)
		{
			var mockRequest = new Mock<HttpRequest>();

			if (string.IsNullOrEmpty(page))
			{
				page = "test";
			}

			if (string.IsNullOrEmpty(login))
			{
				login = "unittestpassword";
			}

			if (string.IsNullOrEmpty(sessionToken))
			{
				sessionToken = Guid.Empty.ToString();
			}

			var query = new Dictionary<string, StringValues>
			{
				{ "page", page },
				{ "input", "INPUT" },
				{ "access-user", "unittestuser" },
				{ "access-token", login },
				{ "session-token", sessionToken },
			};

			mockRequest.Setup(req => req.Query).Returns(new QueryCollection(query));

			return mockRequest.Object;
		}
	}
}

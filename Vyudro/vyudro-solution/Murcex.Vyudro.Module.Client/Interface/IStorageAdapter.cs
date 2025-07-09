namespace Murcex.Vyudro.Module.Client.Interface
{
	public interface IStorageAdapter
	{
		public bool GetPage(string page, out string contents, out string message);

		public bool InsertSessionToken(string token, string timestamp, out string message);

		public bool GetSessionTokenTimestamp(string token, out string timestamp, out string message);

		public bool DeleteSessionToken(string token, out string message);

		public bool GetUserAccess(string manifest, out Dictionary<string, List<string>> userAccess, out string message);
	}
}
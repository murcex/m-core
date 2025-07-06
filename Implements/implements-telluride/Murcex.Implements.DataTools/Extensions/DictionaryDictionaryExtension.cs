namespace Murcex.Implements.DataTools.Extensions
{
	public static class DictionaryDictionaryExtension
	{
		public static Dictionary<string, string> GetValue(
			this Dictionary<string, Dictionary<string, string>> dictionary,
			string key,
			string keyEx = null,
			string valueEx = null,
			bool valueEmpty = false)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (dictionary.TryGetValue(key, out var value))
			{
				if (value == null)
				{
					if (string.IsNullOrEmpty(valueEx))
					{
						if (valueEmpty)
						{
							return new Dictionary<string, string>();
						}
						else
						{
							return null;
						}
					}
					else
					{
						throw new Exception(valueEx);
					}
				}
				else
				{
					return value;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(keyEx))
				{
					if (valueEmpty)
					{
						return new Dictionary<string, string>();
					}
					else
					{
						return null;
					}
				}
				else
				{
					throw new Exception(keyEx);
				}
			}
		}
	}
}

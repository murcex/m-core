namespace Murcex.Implements.DataTools.Extensions
{
	public static class DictionaryExtension
	{

		/// <summary>
		/// Get Dictionary Value with optional exception on missing key and null value
		/// </summary>
		public static string GetValue(this Dictionary<string, string> dictionary, string key, string keyEx = null, string valueEx = null)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (dictionary.TryGetValue(key, out var value))
			{
				if (string.IsNullOrEmpty(value))
				{
					if (string.IsNullOrEmpty(valueEx))
					{
						return string.Empty;
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
					return string.Empty;
				}
				else
				{
					throw new Exception(keyEx);
				}
			}
		}
	}
}
